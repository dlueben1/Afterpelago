using System.IO;
using Microsoft.Win32;

namespace AfterpelagoWPF.Connectors
{
    /// <summary>
    /// Describes how to locate a specific application on the user's system.
    /// </summary>
    /// <param name="ExecutableName">The name of the executable to look for (e.g. "poptracker.exe").</param>
    /// <param name="RegistryDisplayName">
    /// The display name to search for in Add/Remove Programs registry entries.
    /// Pass null to skip registry detection (e.g. for portable apps).
    /// </param>
    /// <param name="StartMenuName">
    /// The name to search for in Start Menu shortcuts.
    /// Pass null to skip Start Menu detection.
    /// </param>
    /// <param name="CommonSubfolderName">The subfolder name appended to each common install location to probe.</param>
    /// <param name="ExtraCommonPaths">Additional paths to check beyond the standard set, if any.</param>
    public sealed record AppDetectorConfig(
        string ExecutableName,
        string? RegistryDisplayName,
        string? StartMenuName,
        string CommonSubfolderName,
        string[] ExtraCommonPaths
    );

    /// <summary>
    /// Generic utility for locating an application directory on the user's system.
    /// Supports registry uninstall entries, Start Menu shortcuts, and common path probing.
    /// Callers supply an <see cref="AppDetectorConfig"/> describing what to look for.
    /// </summary>
    public sealed class AppDetector(AppDetectorConfig config)
    {
        private readonly AppDetectorConfig _config = config;

        /// <summary>
        /// Runs all configured detection strategies in order and returns the first valid path found,
        /// or null if none succeeded.
        /// </summary>
        public string? Detect()
        {
            // 1. Registry (formal installs only)
            if (_config.RegistryDisplayName != null)
            {
                string? registryPath = FindInRegistry(_config.RegistryDisplayName);
                if (registryPath != null)
                    return registryPath;
            }

            // 2. Start Menu shortcuts (catches portable apps that register a shortcut)
            if (_config.StartMenuName != null)
            {
                string? shortcutPath = FindViaStartMenuShortcut(_config.StartMenuName);
                if (shortcutPath != null)
                    return shortcutPath;
            }

            // 3. Common install and portable locations
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string sub = _config.CommonSubfolderName;
            string[] standardPaths =
            [
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), sub),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), sub),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Programs", sub),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), sub),
                Path.Combine(userProfile, "Downloads", sub),
                Path.Combine(userProfile, "Documents", sub),
            ];

            foreach (string path in standardPaths)
                if (IsValidDirectory(path))
                    return path;

            foreach (string path in _config.ExtraCommonPaths)
                if (IsValidDirectory(path))
                    return path;

            // We couldn't find it...
            return null;
        }

        /// <summary>
        /// Returns true if the given path is a directory containing the configured executable.
        /// </summary>
        public bool IsValidDirectory(string path)
        {
            return Directory.Exists(path) &&
                   File.Exists(Path.Combine(path, _config.ExecutableName));
        }

        /// <summary>
        /// Searches both HKLM and HKCU uninstall registry keys for an entry whose DisplayName
        /// contains <paramref name="displayName"/>, and returns its InstallLocation if valid.
        /// </summary>
        private string? FindInRegistry(string displayName)
        {
            string[] registryKeys =
            [
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall",
            ];

            foreach (RegistryKey hive in new[] { Registry.LocalMachine, Registry.CurrentUser })
            {
                foreach (string keyPath in registryKeys)
                {
                    using RegistryKey? key = hive.OpenSubKey(keyPath);
                    if (key == null) continue;

                    foreach (string subKeyName in key.GetSubKeyNames())
                    {
                        using RegistryKey? subKey = key.OpenSubKey(subKeyName);
                        if (subKey == null) continue;

                        string? entryName = subKey.GetValue("DisplayName") as string;
                        if (entryName == null || !entryName.Contains(displayName, StringComparison.OrdinalIgnoreCase))
                            continue;

                        string? installLocation = subKey.GetValue("InstallLocation") as string;
                        if (installLocation != null && IsValidDirectory(installLocation))
                            return installLocation;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Searches Start Menu folders for a .lnk file whose name contains <paramref name="shortcutName"/>,
        /// then resolves the shortcut's target directory.
        /// </summary>
        private string? FindViaStartMenuShortcut(string shortcutName)
        {
            string[] startMenuPaths =
            [
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs"),
            ];

            foreach (string menuPath in startMenuPaths)
            {
                if (!Directory.Exists(menuPath))
                    continue;

                try
                {
                    foreach (string lnkFile in Directory.EnumerateFiles(menuPath, "*.lnk", SearchOption.AllDirectories))
                    {
                        string fileName = Path.GetFileNameWithoutExtension(lnkFile);
                        if (!fileName.Contains(shortcutName, StringComparison.OrdinalIgnoreCase))
                            continue;

                        string? targetDir = ResolveShortcutDirectory(lnkFile);
                        if (targetDir != null && IsValidDirectory(targetDir))
                            return targetDir;
                    }
                }
                catch
                {
                    // Ignore permission or access errors while enumerating
                }
            }

            return null;
        }

        /// <summary>
        /// Resolves a .lnk shortcut file to the directory of its target using the Windows Shell COM API.
        /// </summary>
        private static string? ResolveShortcutDirectory(string lnkPath)
        {
            try
            {
                Type? shellType = Type.GetTypeFromProgID("WScript.Shell");
                if (shellType == null) return null;

                dynamic? shell = Activator.CreateInstance(shellType);
                if (shell == null) return null;

                try
                {
                    dynamic shortcut = shell.CreateShortcut(lnkPath);
                    try
                    {
                        string targetPath = shortcut.TargetPath;
                        if (!string.IsNullOrEmpty(targetPath))
                            return Path.GetDirectoryName(targetPath);
                    }
                    finally
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(shortcut);
                    }
                }
                finally
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(shell);
                }
            }
            catch
            {
                // COM interop can fail in various ways; swallow and move on
            }

            return null;
        }
    }
}
