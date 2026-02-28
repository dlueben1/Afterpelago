namespace AfterpelagoWPF.Connectors
{
    /// <summary>
    /// Detects the PopTracker installation directory on the user's system.
    /// PopTracker is typically a portable application, so registry detection is skipped
    /// in favour of Start Menu shortcuts and common locations.
    /// </summary>
    public static class PopTrackerDetector
    {
        private static readonly AppDetector _detector = new(new AppDetectorConfig(
            ExecutableName: "poptracker.exe",
            RegistryDisplayName: null,
            StartMenuName: "poptracker",
            CommonSubfolderName: "poptracker",
            ExtraCommonPaths: []
        ));

        /// <summary>
        /// Attempts to detect the PopTracker directory.
        /// Checks Start Menu shortcuts and common locations where a portable app might reside.
        /// </summary>
        /// <returns>The path to the PopTracker directory, or null if not found.</returns>
        public static string? Detect() => _detector.Detect();

        /// <summary>
        /// Validates whether a given path is a valid PopTracker directory
        /// by checking for the presence of poptracker.exe.
        /// </summary>
        public static bool IsValidPopTrackerDirectory(string path) => _detector.IsValidDirectory(path);
    }
}
