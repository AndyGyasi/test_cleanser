using System.Security.Cryptography;
using CleanserBlazorUI.Data;

namespace CleanserBlazorUI.Services;

/// <summary>
/// Everything about temporary passwords in one place. An admin gives a new (or locked-out)
/// person a temporary password outside the app; it works for <see cref="Lifetime"/>, and the
/// person must replace it with their own at first sign-in. The app never emails passwords.
/// </summary>
public static class TemporaryPassword
{
    /// <summary>How long a temporary password stays usable after it is set.</summary>
    public static readonly TimeSpan Lifetime = TimeSpan.FromDays(3);

    public static bool IsExpired(ApplicationUser user) =>
        user.TemporaryPasswordSetUtc is { } setAt && DateTime.UtcNow - setAt > Lifetime;

    public static DateTime? ExpiresUtc(ApplicationUser user) => user.TemporaryPasswordSetUtc?.Add(Lifetime);

    // Look-alike characters (0/O, 1/l/I) are left out because the password is read out or typed by hand.
    private const string Upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
    private const string Lower = "abcdefghijkmnpqrstuvwxyz";
    private const string Digits = "23456789";
    private const string Symbols = "!#$%*+-=?@";

    /// <summary>A random 14-character password that satisfies the app's password rules.</summary>
    public static string Generate()
    {
        const int length = 14;
        var chars = new List<char>(length);
        // at least two of each kind, so every rule is met whatever the random draw
        for (var i = 0; i < 2; i++)
        {
            chars.Add(Pick(Upper)); chars.Add(Pick(Lower)); chars.Add(Pick(Digits)); chars.Add(Pick(Symbols));
        }
        var all = Upper + Lower + Digits + Symbols;
        while (chars.Count < length) chars.Add(Pick(all));

        // Fisher-Yates shuffle, so the guaranteed characters are not always at the start
        for (var i = chars.Count - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }
        return new string(chars.ToArray());
    }

    private static char Pick(string alphabet) => alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];
}
