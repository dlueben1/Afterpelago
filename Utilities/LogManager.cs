using Microsoft.AspNetCore.Components.Forms;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Collections.Generic;
using Afterpelago.Models;
using System.Text.RegularExpressions;
using System.Security.AccessControl;

namespace Afterpelago.Utilities
{
    public static class LogManager
    {
        /// <summary>
        /// The raw log file, line-by-line, without alterations from the original file.
        /// @todo Maybe we don't want this long term? Consider removing it later.
        /// </summary>
        public static LogEntry[]? RawLogs { get => rawLogs; }
        private static LogEntry[]? rawLogs;

        #region Log Category Regex Patterns

        /// <summary>
        /// The Factories for each type of Log Entry. Will attempt to match each line against these patterns (in order, ordered by appearance frequency for performance),
        /// then return a valid LogEntry when a match is found.
        /// </summary>
        private static readonly (Regex Pattern, Func<Match, LogEntry> Handler)[] EntryFactories = new (Regex Pattern, Func<Match, LogEntry> Handler)[]
        {
            // Check Sent Entry
            (new Regex(@"^\(Team #(?<team>\d+)\) (?<sender>.+?) sent (?<item>.+?) to (?<receiver>.+?) \((?<location>.+?)\)$", RegexOptions.Compiled),
             (Match m) => new CheckObtainedLogEntry
             {
                 Message = m.Value,
                 Category = LogEntryType.CheckFound,
                 SenderName = m.Groups["sender"].Value,
                 ReceiverName = m.Groups["receiver"].Value,
                 ItemName = m.Groups["item"].Value,
                 LocationName = m.Groups["location"].Value
             }),
            // Player Connection Entry
            (new Regex(@"^Notice \(all\): (?<player>.+?) \(Team #\d+\) playing (?<game>.+?) has joined\. Client\((?<version>\d+\.\d+\.\d+)\), \[(?<array>(?:'[^']*'(?:, )?)*)\]\.$", RegexOptions.Compiled),
             (Match m) => new ConnectionLogEntry
             {
                 Message = m.Value,
                 SlotName = m.Groups["player"].Value,
                 GameName = m.Groups["game"].Value,
                 ClientVersion = m.Groups["version"].Value
             }),
            // Player Disconnection Entry
            (new Regex(@"^Notice \(all\): (?<player>.+?) \(Team #\d+\) has left the game\. Client\(\d+\.\d+\.\d+\), \[\].$", RegexOptions.Compiled),
             (Match m) => new DisconnectLogEntry
             {
                 Message = m.Value,
                 SlotName = m.Groups["player"].Value
             }),
            // Server Shutdown Entry
            (new Regex(@"^Shutting down due to inactivity\.$", RegexOptions.Compiled),
             (Match m) => new LogEntry
             {
                 Message = m.Value,
                 Category = LogEntryType.ServerShutdown
             }),
            // Completion/Release Entry
            (new Regex(@"^Notice \(all\): (?<player>.+?) \(Team #\d+\) has released all remaining items from their world\.$", RegexOptions.Compiled),
             (Match m) => new ReleaseLogEntry
             {
                 Message = m.Value,
                 SlotName = m.Groups["player"].Value
             }),
        };

        #endregion

        #region Reading a Log File

        /// <summary>
        /// Reads a log file line by line and processes all data provided by it.
        /// </summary>
        /// <!-- @todo this is not a well-written description... -->
        /// <!-- @todo cut down on repeated code around total active playtime -->
        /// <!-- @todo in the future there has got to be a way to make dynamic handlers so it isn't a giant function -->
        /// <param name="file">The Log File passed in from the UI</param>
        public static async Task ReadFromFile(IBrowserFile file)
        {
            // Open the file as a StreamReader as process all lines into Afterpelago
            using (var stream = file.OpenReadStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    // Buffer for the current line, which can be null
                    string? line;

                    // Buffer for all lines, which will eventually be converted into an Array for read speed
                    var lines = new List<LogEntry>();

                    // Buffer for all checks, which will eventually be converted into an Array for read speed
                    var checks = new List<Check>();

                    // Buffer to keep track of total seconds of active gameplay, and number of actives players needed to access
                    ulong totalActiveSeconds = 0;
                    var _activePlayers = 0;
                    DateTime _activeStart = DateTime.MinValue;

                    // Keep reading lines until the end of the file
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        // Back out if the line is null
                        if (line == null) continue;

                        // Process the entry to obtain the categorized log entry
                        var entry = ProcessEntry(line);

                        // Back out if the entry cannot be processed (unlikely, but possible)
                        if (entry == null) continue;

                        // Cache the line for the in-memory log file
                        lines.Add(entry);

                        // Parse the entry for any additional data if needed (running this here to cut down on repeated searches, which we do enough of later)
                        switch (entry.Category)
                        {
                            case LogEntryType.PlayerConnection:
                            {
                                // If there are no active players at this point, mark the start time
                                if(_activePlayers == 0) _activeStart = entry.Timestamp;

                                // Keep track of the number of connected players
                                _activePlayers++;

                                // Grab the details, and make sure it's not null (even though it shouldn't be, since we just matched it)
                                var details = entry as ConnectionLogEntry;
                                if (details == null) break;

                                // Attempt to add the Game to the Archipelago Games list
                                if (!Archipelago.Games.ContainsKey(details.GameName))
                                {
                                    Archipelago.Games[details.GameName] = new Game(details.GameName);
                                }

                                // Attempt to add the player/slot to the Archipelago Slots list
                                if (!Archipelago.Slots.ContainsKey(details.SlotName))
                                {
                                    Archipelago.Slots[details.SlotName] = new Slot(details.SlotName, Archipelago.Games[details.GameName]);
                                }

                                break;
                            }
                            case LogEntryType.PlayerDisconnect:
                            {
                                // Decrement the number of connected players
                                _activePlayers--;

                                // If there are no active players, add to the total active time
                                if(_activePlayers == 0)
                                {
                                    var span = entry.Timestamp - _activeStart;
                                    totalActiveSeconds += (ulong)Math.Truncate(span.TotalSeconds);
                                }

                                break;
                            }
                            case LogEntryType.ServerShutdown:
                            {
                                _activePlayers = 0; 
                                var span = entry.Timestamp - _activeStart;
                                _activeStart = entry.Timestamp;
                                totalActiveSeconds += (ulong)Math.Truncate(span.TotalSeconds);
                                break;
                            }
                            case LogEntryType.CheckFound:
                            {
                                // Grab the details, and make sure it's not null (even though it shouldn't be, since we just matched it)
                                var details = entry as CheckObtainedLogEntry;
                                if (details == null) break;

                                // Keep track of the order in which these checks were obtained
                                details.ObtainedOrder = checks.Count + 1;

                                // Cache the check for later
                                checks.Add(details);

                                break;
                            }
                        }
                    }

                    // Once we're at the end of the log file, let's add in the remaining active play time
                    if(_activePlayers > 0)
                    {
                        var finalSpan = lines[lines.Count - 2].Timestamp - _activeStart;
                        totalActiveSeconds += (ulong)Math.Truncate(finalSpan.TotalSeconds);
                    }

                    // Store the total Active Play Time
                    Archipelago.TotalActivePlaytime = TimeSpan.FromSeconds(totalActiveSeconds);

                    // Convert the log file and collection of checks into arrays for faster read access
                    rawLogs = lines.ToArray();
                    Archipelago.Checks = checks.ToArray();
                }
            }
        }

        /// <summary>
        /// Processes a single entry (line) from the log file and attempts to categorize it.
        /// </summary>
        /// <param name="line">The line/entry to parse, as a string</param>
        /// <returns>The Entry and it's data</returns>
        private static LogEntry? ProcessEntry(string line)
        {
            // Attempt to parse the log line using regex
            string pattern = @"^\[(?<timestamp>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2},\d{3})\]: (?<message>.*)$";
            var match = Regex.Match(line, pattern);

            // If we weren't successful, back out
            if (!match.Success) return null;

            // If we were successful, parse the entry
            LogEntry baseEntry = new LogEntry
            {
                Timestamp = DateTime.ParseExact(match.Groups["timestamp"].Value, "yyyy-MM-dd HH:mm:ss,fff", null),
                Message = match.Groups["message"].Value,
                Category = LogEntryType.Unknown
            };

            // Categorize the Log Entry based on its message content
            for(int i = 0; i < EntryFactories.Length; i++)
            {
                // Grab the pattern and handler
                var (Pattern, Factory) = EntryFactories[i];

                // Check if the log entry matches the pattern
                var entryMatch = Pattern.Match(baseEntry.Message);
                if(entryMatch.Success)
                {
                    // Build the categorized entry from it's factory function
                    var categorizedEntry = Factory(entryMatch);

                    // Pass in the timestamp from the base entry
                    categorizedEntry.Timestamp = baseEntry.Timestamp;

                    return categorizedEntry;
                }
            }

            // If we found no entries, return the base entry
            return baseEntry;
        }

        #endregion
    }
}
