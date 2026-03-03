using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AfterpelagoWPF.Models
{
    /// <summary>
    /// Represents a game discovered in an Archipelago log file.
    /// </summary>
    public class GameEntry : INotifyPropertyChanged
    {
        private string _gameName = string.Empty;
        private bool _isSupported;
        private string? _apWorldFilePath;
        private string? _zipFilePath;
        private string _statusText = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// The display name of the game as found in the log.
        /// </summary>
        public string GameName
        {
            get => _gameName;
            set { _gameName = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Whether Afterpelago has local game data for this game
        /// (i.e. a directory exists in %LocalAppData%/Afterpelago/games).
        /// </summary>
        public bool IsSupported
        {
            get => _isSupported;
            set { _isSupported = value; OnPropertyChanged(); OnPropertyChanged(nameof(NeedsConfiguration)); }
        }

        /// <summary>
        /// True when the game is not locally supported and requires user-provided files.
        /// </summary>
        public bool NeedsConfiguration => !IsSupported;

        /// <summary>
        /// A short description of the game's configuration status.
        /// </summary>
        public string StatusText
        {
            get => _statusText;
            set { _statusText = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Path to the user-provided .apworld file (for unsupported games).
        /// </summary>
        public string? ApWorldFilePath
        {
            get => _apWorldFilePath;
            set { _apWorldFilePath = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Path to the user-provided .zip file (for unsupported games).
        /// </summary>
        public string? ZipFilePath
        {
            get => _zipFilePath;
            set { _zipFilePath = value; OnPropertyChanged(); }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
