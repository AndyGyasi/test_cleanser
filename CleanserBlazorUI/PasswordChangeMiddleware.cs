/// <summary>
/// Every newly created account carries the "MustChangePassword" role until its
/// owner picks their own password. Until then the only things they can reach are
/// the set-password page, log out, and the static/framework files those pages
/// need -- every other request is sent to the set-password page, so the
/// requirement can't be skipped by typing another address.
///
/// The role is read from the sign-in cookie's claims (no database hit per
/// request); ChangeTemporaryPassword refreshes the cookie when the role is lifted.
/// </summary>
public class PasswordChangeMiddleware(RequestDelegate next)
{
    public const string RoleName = "MustChangePassword";
    public const string SetPasswordPath = "/Account/ChangeTemporaryPassword";

    private static readonly string[] AllowedPrefixes =
    {
        SetPasswordPath, "/Account/Logout", "/session/", "/_framework", "/_blazor", "/_content", "/api/uploads"
    };

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true && context.User.IsInRole(RoleName))
        {
            var path = context.Request.Path;
            var allowed = Path.HasExtension(path.Value) // css/js/images/fonts
                || AllowedPrefixes.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase) || path.Value!.StartsWith(p, StringComparison.OrdinalIgnoreCase));

            if (!allowed)
            {
                context.Response.Redirect(SetPasswordPath);
                return;
            }
        }

        await next(context);
    }
}
