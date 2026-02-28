using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using AfterpelagoWPF.Connectors;
using AfterpelagoWPF.Services;
using Microsoft.Win32;

namespace AfterpelagoWPF.Views
{
    /// <summary>
    /// Lets a user specify their local Archipelago Installation
    /// </summary>
    public partial class ArchipelagoConfigDialog : Window, INotifyPropertyChanged
    {
        private string? _currentPath;
        private bool _isValidArchipelagoDirectory;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Displays the Current Path for a user's Archipelago Installation.
        /// If no installation is found, a default message is shown.
        /// </summary>
        public string? CurrentPath
        {
            get => _currentPath ?? "No Archipelago installation found.";
            set
            {
                if (_currentPath != value)
                {
                    _currentPath = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Whether or not the Directory is a valid Archipelago Directory or not.
        /// </summary>
        public bool IsValidArchipelagoDirectory
        {
            get => _isValidArchipelagoDirectory;
            set
            {
                if (_isValidArchipelagoDirectory != value)
                {
                    _isValidArchipelagoDirectory = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// WPF Binding helper to trigger UI updates when properties change
        /// </summary>
        /// <param name="propertyName">The name of the property that changed</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Initialization of the dialog. Sets up data context and attempts to auto-detect Archipelago installation.
        /// </summary>
        public ArchipelagoConfigDialog()
        {
            InitializeComponent();
            DataContext = this;
            DetectArchipelago();
        }

        /// <summary>
        /// Tries to find the user's Archipelago Installation, then updates the UI state.
        /// </summary>
        private void DetectArchipelago()
        {
            var detectedPath = ArchipelagoDetector.Detect();

            if (detectedPath != null)
            {
                CurrentPath = detectedPath;
                IsValidArchipelagoDirectory = true;
            }
            else
            {
                CurrentPath = null;
                IsValidArchipelagoDirectory = false;
            }
        }

        /// <summary>
        /// Saves the Archipelago Path to the App Settings. 
        /// If the path is invalid, prompts the user for confirmation before saving.
        /// </summary>
        private void SavePathButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidArchipelagoDirectory)
            {
                var result = MessageBox.Show(
                    "The selected folder does not appear to contain ArchipelagoLauncher.exe. Use it anyway?",
                    "Invalid Directory",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            SavePath(_currentPath);
        }

        /// <summary>
        /// Allows the user to manually select their Archipelago Installation.
        /// Used when auto-detection fails or the User has multiple Installations and wants to specify which one to use.
        /// </summary>
        private void ManualChooseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select your Archipelago installation directory"
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedPath = dialog.FolderName;
                CurrentPath = selectedPath;
                IsValidArchipelagoDirectory = ArchipelagoDetector.IsValidArchipelagoDirectory(selectedPath);
            }
        }

        /// <summary>
        /// Saves the Archipelago Path to App Settings and closes the dialog.
        /// </summary>
        /// <param name="path">The path to the user's Archipelago Installation</param>
        private void SavePath(string? path)
        {
            AppSettingsService.Values.ArchipelagoPath = path;
            AppSettingsService.Save();

            DialogResult = true;
            Close();
        }
    }
}
