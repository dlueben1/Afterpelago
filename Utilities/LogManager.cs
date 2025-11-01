using Microsoft.AspNetCore.Components.Forms;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Collections.Generic;

namespace Afterpelago.Utilities
{
    public static class LogManager
    {
        /// <summary>
        /// The raw log file, line-by-line, without alterations from the original file.
        /// </summary>
        public static string[]? RawLogs { get => rawLogs; }
        private static string[]? rawLogs;

        /// <summary>
        /// Reads a log file line by line and processes all data provided by it.
        /// </summary>
        /// <!-- @todo this is not a well-written description... -->
        /// <param name="file">The Log File passed in from the UI</param>
        public static async Task ReadFromFile(IBrowserFile file)
        {
            // Open the file as a StreamReader
            using (var stream = file.OpenReadStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    // Buffer for the current line, which can be null
                    string? line;

                    // Buffer for all lines, which will eventually be converted into an Array for read speed
                    var lines = new List<string>();

                    // Keep reading lines until the end of the file
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        if(line != null) lines.Add(line);
                    }

                    // Convert the log file into an array for faster read access
                    rawLogs = lines.ToArray();
                }
            }
        }
    }
}
