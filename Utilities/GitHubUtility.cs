using Afterpelago.Models;
using Afterpelago.Models.Responses.GitHub;
using ICSharpCode.SharpZipLib.Zip;
using System.Net.Http.Json;

namespace Afterpelago.Utilities
{
    public static class GitHubUtility
    {
        public static async Task DownloadTrackerData(Game game)
        {
            // Ensure we have what we need to proceed
            if (game == null || string.IsNullOrEmpty(game.APTrackerSource))
            {
                throw new ArgumentException("Invalid game data.");
            }

            // The HTTP Client is used for standard API requests
            using (var httpClient = new HttpClient())
            {
                // Step 1: Grab information about the latest release
                var info = await httpClient.GetFromJsonAsync<GitHubReleasesInfoResponse>($"https://api.github.com/repos/{game.APTrackerSource}/releases/latest");
                if (info == null || string.IsNullOrEmpty(info.Zipball_URL))
                {
                    throw new Exception("Failed to retrieve Zipball URL from GitHub.");
                }

                // Step 2: Download the ZIP from the release info as a Byte Array
                var zipData = await httpClient.GetByteArrayAsync(info.Zipball_URL);

                // Step 3: Push the array into a MemoryStream and extract `items.json`
                using (MemoryStream ms = new MemoryStream(zipData))
                {
                    using (ZipFile zip = new ZipFile(ms))
                    {
                        foreach(var file in zip)
                        {
                            Console.WriteLine("ZIP: " + file.ToString());
                        }
                    }
                }
            }
        }
    }
}
