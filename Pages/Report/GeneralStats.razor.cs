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

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // Calculate and clean statistics
            ActivePlayTime = TimeUtilities.TimespanToReadableString(Archipelago.TotalActivePlaytime);

            var _firstCheck = Archipelago.Checks[0];
            FirstCheck = $"{_firstCheck.SenderName}: {_firstCheck.LocationName}";
            FirstCheckClear = _firstCheck.Timestamp.ToString();
            
            var _lastCheck = Archipelago.Checks[Archipelago.Checks.Length - 1];
            LastCheck = $"{_lastCheck.SenderName}: {_lastCheck.LocationName}";
            LastCheckClear = _lastCheck.Timestamp.ToString();
        }
    }
}
