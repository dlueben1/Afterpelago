using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using AfterpelagoWPF.Connectors;
using AfterpelagoWPF.Models;
using Microsoft.Win32;

namespace AfterpelagoWPF.Views
{
    /// <summary>
    /// A two-step wizard for creating a new Archipelago session report.
    /// Step 1: Select log file (required) and spoiler log (optional).
    /// Step 2: Review discovered games and optionally provide data files for unsupported ones.
    /// </summary>
    public partial class ReportWizard : UserControl
    {
        private int _currentStep = 1;
        private string? _logFilePath;
        private string? _spoilerFilePath;

        /// <summary>
        /// The collection of games discovered after importing the log file.
        /// </summary>
        public ObservableCollection<GameEntry> DiscoveredGames { get; } = new();

        /// <summary>
        /// Raised when the user clicks "Back" on Step 1, signaling the host to navigate away.
        /// </summary>
        public event EventHandler? NavigateBack;

        public ReportWizard()
        {
            InitializeComponent();
            GamesListBox.ItemsSource = DiscoveredGames;
            UpdateStepVisuals();
        }

        #region File Browsing

        private void OnBrowseLogFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Archipelago Log File",
                Filter = "Log files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                _logFilePath = dialog.FileName;
                LogFilePathTextBox.Text = dialog.FileName;
                UpdateButtonStates();
            }
        }

        private void OnBrowseSpoilerFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Spoiler Log",
                Filter = "Spoiler logs (*.txt;*.json)|*.txt;*.json|All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                _spoilerFilePath = dialog.FileName;
                SpoilerFilePathTextBox.Text = dialog.FileName;
            }
        }

        private void OnBrowseApWorld(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is GameEntry game)
            {
                var dialog = new OpenFileDialog
                {
                    Title = $"Select .apworld file for {game.GameName}",
                    Filter = "AP World files (*.apworld)|*.apworld|All files (*.*)|*.*"
                };

                if (dialog.ShowDialog() == true)
                {
                    game.ApWorldFilePath = dialog.FileName;
                    game.StatusText = BuildStatusText(game);
                }
            }
        }

        private void OnBrowseZip(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is GameEntry game)
            {
                var dialog = new OpenFileDialog
                {
                    Title = $"Select .zip data file for {game.GameName}",
                    Filter = "Zip archives (*.zip)|*.zip|All files (*.*)|*.*"
                };

                if (dialog.ShowDialog() == true)
                {
                    game.ZipFilePath = dialog.FileName;
                    game.StatusText = BuildStatusText(game);
                }
            }
        }

        #endregion

        #region Navigation

        private void OnBack(object sender, RoutedEventArgs e)
        {
            if (_currentStep == 2)
            {
                _currentStep = 1;
                UpdateStepVisuals();
            }
            else
            {
                NavigateBack?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnImport(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_logFilePath))
                return;

            // TODO: Call LogImporter to parse the log file and populate DiscoveredGames

            //DiscoveredGames.Clear();
            //foreach (var name in gameNames)
            //{
            //    var entry = new GameEntry
            //    {
            //        GameName = name,
            //        IsSupported = IsGameSupportedLocally(name)
            //    };
            //    entry.StatusText = BuildStatusText(entry);
            //    DiscoveredGames.Add(entry);
            //}

            //_currentStep = 2;
            //UpdateStepVisuals();
        }

        private void OnGenerate(object sender, RoutedEventArgs e)
        {
            // TODO: Implement report generation logic
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Checks whether Afterpelago has a local game data directory for the given game.
        /// The directory is expected at %LocalAppData%/Afterpelago/games/{name_lower_no_spaces}.
        /// </summary>
        private static bool IsGameSupportedLocally(string gameName)
        {
            string sanitized = gameName.ToLowerInvariant().Replace(" ", "");
            string gamesDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Afterpelago", "games", sanitized);
            return Directory.Exists(gamesDir);
        }

        private static string BuildStatusText(GameEntry game)
        {
            if (game.IsSupported)
                return "Game data found locally.";

            var parts = new List<string>();
            if (!string.IsNullOrEmpty(game.ApWorldFilePath))
                parts.Add(".apworld provided");
            if (!string.IsNullOrEmpty(game.ZipFilePath))
                parts.Add(".zip provided");

            return parts.Count > 0
                ? string.Join(", ", parts)
                : "No local game data — report will have limited information.";
        }

        private void UpdateStepVisuals()
        {
            // Step content visibility
            Step1Content.Visibility = _currentStep == 1 ? Visibility.Visible : Visibility.Collapsed;
            Step2Content.Visibility = _currentStep == 2 ? Visibility.Visible : Visibility.Collapsed;

            // Step indicator styling
            Step1Circle.Style = (Style)Resources[_currentStep >= 1 ? "StepCircleActive" : "StepCircleInactive"];
            Step1Number.Style = (Style)Resources[_currentStep >= 1 ? "StepNumberActive" : "StepNumberInactive"];
            Step1Label.Style = (Style)Resources[_currentStep >= 1 ? "StepLabelActive" : "StepLabelInactive"];

            Step2Circle.Style = (Style)Resources[_currentStep >= 2 ? "StepCircleActive" : "StepCircleInactive"];
            Step2Number.Style = (Style)Resources[_currentStep >= 2 ? "StepNumberActive" : "StepNumberInactive"];
            Step2Label.Style = (Style)Resources[_currentStep >= 2 ? "StepLabelActive" : "StepLabelInactive"];

            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool hasLogFile = !string.IsNullOrEmpty(_logFilePath);

            // Import button: visible and enabled only on Step 1 when a log file is selected
            ImportButton.Visibility = _currentStep == 1 ? Visibility.Visible : Visibility.Collapsed;
            ImportButton.IsEnabled = hasLogFile;

            // Generate button: visible and enabled only on Step 2
            GenerateButton.Visibility = _currentStep == 2 ? Visibility.Visible : Visibility.Collapsed;
            GenerateButton.IsEnabled = _currentStep == 2;
        }

        #endregion
    }
}
