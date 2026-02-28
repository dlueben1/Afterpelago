using System.IO;
using System.Text.Json;

namespace AfterpelagoWPF.Services
{
    /// <summary>
    /// Serializable container for all application settings values
    /// </summary>
    public sealed class AppSettingsValues
    {
        /// <summary>
        /// Gets or sets the path to the Archipelago installation directory
        /// </summary>
        public string? ArchipelagoPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the PopTracker installation directory
        /// </summary>
        public string? PopTrackerPath { get; set; }
    }

    /// <summary>
    /// Manages application settings with static access pattern
    /// </summary>
    public static class AppSettingsService
    {
        #region Constants

        /// <summary>
        /// The path to the settings directory in Local AppData
        /// </summary>
        private static readonly string SettingsDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Afterpelago");

        /// <summary>
        /// The name of the settings file
        /// </summary>
        private static readonly string SettingsFile = Path.Combine(SettingsDir, "settings.json");

        #endregion

        #region Singleton Instance

        private static readonly Lazy<AppSettingsValues> _values = new(LoadSettings);

        /// <summary>
        /// Gets the current settings values
        /// </summary>
        public static AppSettingsValues Values => _values.Value;

        #endregion

        #region I/O Handling

        /// <summary>
        /// Loads the Archipelago Settings from the Local AppData Folder. 
        /// If the file doesn't exist or is invalid, it returns a new instance with default values.
        /// </summary>
        private static AppSettingsValues LoadSettings()
        {
            if (!File.Exists(SettingsFile))
                return new AppSettingsValues();

            try
            {
                string json = File.ReadAllText(SettingsFile);
                var settings = JsonSerializer.Deserialize<AppSettingsValues>(json);
                if (settings != null)
                {
                    return settings;
                }
            }
            catch
            {
            }

            return new AppSettingsValues();
        }

        /// <summary>
        /// Saves the current settings to the Local AppData Folder
        /// </summary>
        public static void Save()
        {
            Directory.CreateDirectory(SettingsDir);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(Values, options);
            File.WriteAllText(SettingsFile, json);
        }

        /// <summary>
        /// Reloads settings from disk, discarding any unsaved changes
        /// </summary>
        public static void Reload()
        {
            var loaded = LoadSettings();
            Values.ArchipelagoPath = loaded.ArchipelagoPath;
            Values.PopTrackerPath = loaded.PopTrackerPath;
        }

        #endregion
    }
}
