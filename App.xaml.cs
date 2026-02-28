using System.IO;
using System.Windows;

namespace AfterpelagoWPF
{
    public partial class App : Application
    {
        private const string AppName = "Afterpelago";

        /// <summary>
        /// Handle App Startup to create necessary folders in %LocalAppData% for logs and cache.
        /// </summary>
        /// <param name="e">Startup event arguments</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            // Let WPF do it's thing first
            base.OnStartup(e);

            // 1. Get the path to %LocalAppData%
            string localAppPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            // 2. Define your app's specific directory
            string appDataFolder = Path.Combine(localAppPath, AppName);

            // 3. Create the app's directories if they don't exist
            string[] subDirs = new string[]
            {
                "games",
                "cache"
            };
            try
            {
                Directory.CreateDirectory(appDataFolder);
                foreach(var subDir in subDirs)
                {
                    Directory.CreateDirectory(Path.Combine(appDataFolder, subDir));
                }
            }
            catch (Exception ex)
            {
                // Simple error handling for bootup
                MessageBox.Show($"Failed to initialize folders: {ex.Message}", "Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}