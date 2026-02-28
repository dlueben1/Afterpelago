using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using AfterpelagoWPF.Connectors;
using AfterpelagoWPF.Services;
using Microsoft.Win32;

namespace AfterpelagoWPF.Views
{
    /// <summary>
    /// Lets a user specify their local PopTracker Installation
    /// </summary>
    public partial class PopTrackerConfigDialog : Window, INotifyPropertyChanged
    {
        private string? _currentPath;
        private bool _isValidPopTrackerDirectory;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Displays the Current Path for a user's PopTracker Installation.
        /// If no installation is found, a default message is shown.
        /// </summary>
        public string? CurrentPath
        {
            get => _currentPath ?? "No PopTracker installation found.";
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
        /// Whether or not the Directory is a valid PopTracker Directory or not.
        /// </summary>
        public bool IsValidPopTrackerDirectory
        {
            get => _isValidPopTrackerDirectory;
            set
            {
                if (_isValidPopTrackerDirectory != value)
                {
                    _isValidPopTrackerDirectory = value;
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
        /// Initialization of the dialog. Sets up data context and attempts to auto-detect PopTracker installation.
        /// </summary>
        public PopTrackerConfigDialog()
        {
            InitializeComponent();
            DataContext = this;
            DetectPopTracker();
        }

        /// <summary>
        /// Tries to find the user's PopTracker Installation, then updates the UI state.
        /// </summary>
        private void DetectPopTracker()
        {
            var detectedPath = PopTrackerDetector.Detect();

            if (detectedPath != null)
            {
                CurrentPath = detectedPath;
                IsValidPopTrackerDirectory = true;
            }
            else
            {
                CurrentPath = null;
                IsValidPopTrackerDirectory = false;
            }
        }

        /// <summary>
        /// Saves the PopTracker Path to the App Settings. 
        /// If the path is invalid, prompts the user for confirmation before saving.
        /// </summary>
        private void SavePathButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsValidPopTrackerDirectory)
            {
                var result = MessageBox.Show(
                    "The selected folder does not appear to contain poptracker.exe. Use it anyway?",
                    "Invalid Directory",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            SavePath(_currentPath);
        }

        /// <summary>
        /// Allows the user to manually select their PopTracker Installation.
        /// Used when auto-detection fails or the User has multiple installations and wants to specify which one to use.
        /// </summary>
        private void ManualChooseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select your PopTracker installation directory"
            };

            if (dialog.ShowDialog() == true)
            {
                string selectedPath = dialog.FolderName;
                CurrentPath = selectedPath;
                IsValidPopTrackerDirectory = PopTrackerDetector.IsValidPopTrackerDirectory(selectedPath);
            }
        }

        /// <summary>
        /// Saves the PopTracker Path to App Settings and closes the dialog.
        /// </summary>
        /// <param name="path">The path to the user's PopTracker Installation</param>
        private void SavePath(string? path)
        {
            AppSettingsService.Values.PopTrackerPath = path;
            AppSettingsService.Save();

            DialogResult = true;
            Close();
        }
    }
}
