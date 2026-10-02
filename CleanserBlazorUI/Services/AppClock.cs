// The one place the app asks "what time is it?".
//
// Every date and time the app records or shows (when a run was made, when a request was raised, when a
// temporary password was issued ...) comes from here. It is the current moment converted to Ghana time
// (Africa/Accra, GMT, no daylight saving) -- NOT the local clock setting of whichever server the site or a
// database happens to run on, and never a database server's clock (the app asks no database for the time).
public static class AppClock
{
    private static readonly TimeZoneInfo Accra = FindAccra();

    private static TimeZoneInfo FindAccra()
    {
        foreach (var id in new[] { "Africa/Accra", "Greenwich Standard Time" })
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById(id); } catch (TimeZoneNotFoundException) { } catch (InvalidTimeZoneException) { }
        }
        return TimeZoneInfo.Utc;   // Accra is GMT all year, so UTC is the same wall-clock time
    }

    /// <summary>The current date and time in Ghana (GMT).</summary>
    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, Accra);

    /// <summary>Today's date in Ghana.</summary>
    public static DateTime Today => Now.Date;
}
