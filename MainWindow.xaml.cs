using System.IO;
using System.Windows;
using System.Windows.Controls;
using AfterpelagoWPF.Views;
using MahApps.Metro.Controls;

namespace AfterpelagoWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnFileMenuClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.ContextMenu is { } menu)
            {
                menu.PlacementTarget = btn;
                menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                menu.IsOpen = true;
            }
        }

        private void OnToolsMenuClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.ContextMenu is { } menu)
            {
                menu.PlacementTarget = btn;
                menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                menu.IsOpen = true;
            }
        }

        private void OnDebugMenuClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.ContextMenu is { } menu)
            {
                menu.PlacementTarget = btn;
                menu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                menu.IsOpen = true;
            }
        }

        private void OnConfigureArchipelago(object sender, RoutedEventArgs e)
        {
            var dialog = new ArchipelagoConfigDialog
            {
                Owner = this
            };
            dialog.ShowDialog();
        }

        private void OnConfigurePopTracker(object sender, RoutedEventArgs e)
        {
            var dialog = new PopTrackerConfigDialog
            {
                Owner = this
            };
            dialog.ShowDialog();
        }

        /// <summary>
        /// Opens the Local Files for Afterpelago
        /// </summary>
        private void OnBrowseLocalApplications(object sender, RoutedEventArgs e)
        {
            string localAppPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = Path.Combine(localAppPath, "Afterpelago"),
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }
}