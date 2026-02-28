namespace AfterpelagoWPF.Connectors
{
    /// <summary>
    /// Detects the Archipelago installation directory on the user's system.
    /// Archipelago is a formally installed application, so registry uninstall entries are checked first.
    /// </summary>
    public static class ArchipelagoDetector
    {
        private static readonly AppDetector _detector = new(new AppDetectorConfig(
            ExecutableName: "ArchipelagoLauncher.exe",
            RegistryDisplayName: "Archipelago",
            StartMenuName: null,
            CommonSubfolderName: "Archipelago",
            ExtraCommonPaths: []
        ));

        /// <summary>
        /// Attempts to detect the Archipelago installation directory.
        /// Checks the Windows registry uninstall entries and common install paths.
        /// </summary>
        /// <returns>The path to the Archipelago directory, or null if not found.</returns>
        public static string? Detect() => _detector.Detect();

        /// <summary>
        /// Validates whether a given path is a valid Archipelago installation directory
        /// by checking for the presence of ArchipelagoLauncher.exe.
        /// </summary>
        public static bool IsValidArchipelagoDirectory(string path) => _detector.IsValidDirectory(path);
    }
}
