using Microsoft.AspNetCore.Identity;

namespace CleanserBlazorUI.Data
{
    public class ApplicationUser : IdentityUser
    {
        // RETIRED -- nothing set this true/false or read it for real. New accounts are
        // flagged with the "MustChangePassword" *role* instead (see Register.razor and
        // PasswordChangeMiddleware). The AspNetUsers.MustChangePassword column still
        // exists (NOT NULL, no default), so ApplicationDbContext keeps it mapped as a
        // hidden shadow property; nothing can read it.
        // public bool MustChangePassword { get; set; } = false;

        // When the account's current *temporary* password was set (at registration, or by an
        // admin's Reset password). Null once the person has chosen their own. A temporary
        // password stops working TemporaryPassword.Lifetime after this moment.
        public DateTime? TemporaryPasswordSetUtc { get; set; }

        // Corresponds to Ring.Users.UserID and Transact.ReceivedTrans.AssignTo
        // in the XDSDataLogDB database -- identifies which files (by filename,
        // via Transact.ReceivedTrans) this person is allowed to run through
        // the cleanser. Set manually for legacy accounts; auto-populated from
        // Ring.Users for new accounts created via Register.razor.
        public string? ReceivedTransUserID { get; set; }
    }

}
