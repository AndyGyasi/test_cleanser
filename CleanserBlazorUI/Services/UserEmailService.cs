using System.ComponentModel.DataAnnotations;
using CleanserBlazorUI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanserBlazorUI.Services;

/// <summary>
/// An administrator changing the email address someone signs in with. In this app the email IS the
/// username, so both change together, and the person's other records follow them:
/// access requests (looked up by the requester's email, so approvals keep working) and their open
/// password-reset request. Emails are for sign-in only: the link to the file-ownership record
/// (ReceivedTransUserID, which matches Transact.ReceivedTrans.AssignTo and Ring.Users.ID) is by ID,
/// so a change here never affects it and nothing is looked up from the new address.
/// Audit history (who reviewed / attempted / performed something) keeps the address used at the time.
/// </summary>
public class UserEmailService(
    UserManager<ApplicationUser> users,
    ApplicationDbContext db,
    ILogger<UserEmailService> logger)
{
    private const string ProtectedAccount = "noreply.XDSmonitor@XDSdatagh.com";

    /// <summary>Returns an error message to show the admin, or null when the email was changed.</summary>
    public async Task<string?> ChangeAsync(ApplicationUser user, string newEmail, string changedBy)
    {
        newEmail = newEmail.Trim();
        var oldEmail = user.Email ?? user.UserName ?? "";

        if (string.Equals(oldEmail, ProtectedAccount, StringComparison.OrdinalIgnoreCase))
            return "That account cannot be changed.";
        if (!new EmailAddressAttribute().IsValid(newEmail))
            return "Enter a valid email address.";
        if (string.Equals(oldEmail, newEmail, StringComparison.Ordinal))
            return null;

        var taken = await users.FindByEmailAsync(newEmail);
        if (taken is not null && taken.Id != user.Id)
            return "That email address already belongs to another account.";

        await using var tx = await db.Database.BeginTransactionAsync();

        user.Email = newEmail;
        user.UserName = newEmail;
        var result = await users.UpdateAsync(user);   // validates, and refreshes the normalised copies
        if (!result.Succeeded)
        {
            await tx.RollbackAsync();
            await db.Entry(user).ReloadAsync();
            return string.Join(" ", result.Errors.Select(e => e.Description));
        }

        // Ends the person's current sessions shortly, so nothing keeps running under the old address.
        await users.UpdateSecurityStampAsync(user);

        await db.DataLoggingAccessRequests.Where(r => r.RequestedByEmail == oldEmail)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.RequestedByEmail, newEmail));
        await db.PasswordResetRequests.Where(r => r.UserId == user.Id && r.ResolvedUtc == null)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.Email, newEmail));

        await tx.CommitAsync();

        logger.LogInformation("{Admin} changed the email of '{Old}' to '{New}'.", changedBy, oldEmail, newEmail);
        return null;
    }
}
