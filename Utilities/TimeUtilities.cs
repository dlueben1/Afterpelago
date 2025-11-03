namespace Afterpelago.Utilities
{
    public static class TimeUtilities
    {
        public static string TimespanToReadableString(TimeSpan ts)
        {
            var parts = new List<string>();
            if (ts.Days > 0)
                parts.Add($"{ts.Days} day{(ts.Days == 1 ? "" : "s")}");
            if (ts.Hours > 0)
                parts.Add($"{ts.Hours} hour{(ts.Hours == 1 ? "" : "s")}");
            if (ts.Minutes > 0)
                parts.Add($"{ts.Minutes} minute{(ts.Minutes == 1 ? "" : "s")}");
            if (ts.Seconds > 0 || parts.Count == 0) // Always show seconds if everything else is zero
                parts.Add($"{ts.Seconds} second{(ts.Seconds == 1 ? "" : "s")}");
            return string.Join(", ", parts);
        }
    }
}
