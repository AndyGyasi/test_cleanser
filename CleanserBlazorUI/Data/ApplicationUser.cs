using Microsoft.AspNetCore.Identity;

namespace CleanserBlazorUI.Data
{
    public class ApplicationUser : IdentityUser
    {
        public bool MustChangePassword { get; set; } = false;

        // Corresponds to Ring.Users.UserID and Transact.ReceivedTrans.AssignTo
        // in the XDSDataLogDB database -- identifies which files (by filename,
        // via Transact.ReceivedTrans) this person is allowed to run through
        // the cleanser. Set manually for legacy accounts; auto-populated from
        // Ring.Users for new accounts created via Register.razor.
        public string? ReceivedTransUserID { get; set; }
    }

}
