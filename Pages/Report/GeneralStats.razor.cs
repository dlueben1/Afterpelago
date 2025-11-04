using Afterpelago.Utilities;

namespace Afterpelago.Pages.Report
{
    public partial class GeneralStats
    {
        public string StartTime = LogManager.RawLogs[0].Timestamp.ToString();
        public string EndTime = LogManager.RawLogs[LogManager.RawLogs.Length - 1].Timestamp.ToString();

        /// <summary>
        /// The amount of time that people were actually playing the game
        /// </summary>
        public string ActivePlayTime { get; set; }

        public string FirstCheck { get; set; }
        public string FirstCheckClear { get; set; }
        public string LastCheck { get; set; }
        public string LastCheckClear { get; set; }

        public string FastestPayoffText
        {
            get
            {
                return Statistics.Hints.FastestPayoff != null
                    ? $"{Statistics.Hints.FastestPayoff.ReceiverName}'s {Statistics.Hints.FastestPayoff.ItemName} took {Statistics.Hints.FastestPayoff.SenderName} {Statistics.Hints.TimeToFulfillHints[Statistics.Hints.FastestPayoff.UniqueId].ToReadableString()} to fulfill upon seeing the hint"
                    : "N/A";
            }
        }

        public string LongestPayoffText
        {
            get
            {
                return Statistics.Hints.LongestPayoff != null
                    ? $"{Statistics.Hints.LongestPayoff.ReceiverName}'s {Statistics.Hints.LongestPayoff.ItemName} took {Statistics.Hints.LongestPayoff.SenderName} {Statistics.Hints.TimeToFulfillHints[Statistics.Hints.LongestPayoff.UniqueId].ToReadableString()} to fulfill upon seeing the hint"
                    : "N/A";
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // Calculate and clean statistics
            ActivePlayTime = Archipelago.TotalActivePlaytime.ToReadableString();

            var _firstCheck = Archipelago.Checks[0];
            FirstCheck = $"{_firstCheck.SenderName}: {_firstCheck.LocationName}";
            FirstCheckClear = _firstCheck.Timestamp.ToString();
            
            var _lastCheck = Archipelago.Checks[Archipelago.Checks.Length - 1];
            LastCheck = $"{_lastCheck.SenderName}: {_lastCheck.LocationName}";
            LastCheckClear = _lastCheck.Timestamp.ToString();
        }
    }
}
