using Afterpelago.Models;
using ApexCharts;

namespace Afterpelago.Trackers
{
    public class ChecksPerHourTracker : ITracker
    {
        private const long _hoursToMilliseconds = 3600000;
        Dictionary<string, int> checksPerPlayer = new Dictionary<string, int>();
        Dictionary<string, long> millisecondsActivePerPlayer = new Dictionary<string, long>();
        Dictionary<string, bool> isOnlinePerPlayer = new Dictionary<string, bool>();
        Dictionary<string, DateTime> lastLoginPerPlayer = new Dictionary<string, DateTime>();
        public void ParseLine(LogEntry entry)
        {
            TrackCheckFound(entry);
            TrackTimeActive(entry);
        }

        private void TrackCheckFound(LogEntry entry)
        {
            // Only care about Checks
            if (entry.Category != LogEntryType.CheckFound) return;
            var check = entry as Check;
            if (check == null) return;

            // Increment the number of checks found
            if (checksPerPlayer.ContainsKey(check.SenderName))
            {
                checksPerPlayer[check.SenderName]++;
            }
            else
            {
                checksPerPlayer[check.SenderName] = 1;
            }
        }

        private void TrackTimeActive(LogEntry entry)
        {
            // We care about logins, logouts, and shutdowns
            switch(entry.Category)
            {
                case LogEntryType.PlayerConnection:
                    {
                        // Get the connection entry
                        var connection = entry as ConnectionLogEntry;
                        if(connection == null) return;

                        // Mark the player as "online"
                        isOnlinePerPlayer[connection.SlotName] = true;

                        // Track this login
                        lastLoginPerPlayer[connection.SlotName] = entry.Timestamp;
                        break;
                    }
                case LogEntryType.PlayerDisconnect:
                    {
                        // Get the disconnection entry
                        var disconnect = entry as DisconnectLogEntry;
                        if(disconnect == null) return;

                        // If this player wasn't online, back out
                        if(!isOnlinePerPlayer.ContainsKey(disconnect.SlotName) || !isOnlinePerPlayer[disconnect.SlotName]) return;

                        // Add milliseconds of online time
                        if (!lastLoginPerPlayer.ContainsKey(disconnect.SlotName)) return;
                        HandleDisconnection(entry.Timestamp, disconnect.SlotName);

                        break;
                    }
                case LogEntryType.ServerShutdown:
                    {
                        // Set all players as offline
                        foreach(var player in isOnlinePerPlayer.Keys)
                        {
                            // Handle disconnection
                            if(isOnlinePerPlayer[player])
                                HandleDisconnection(entry.Timestamp, player);
                        }

                        break;
                    }
            }
        }

        private void HandleDisconnection(DateTime disconnectTime, string slotName)
        {
            long onlineTime = disconnectTime.ToUnixTimeMilliseconds() - lastLoginPerPlayer[slotName].ToUnixTimeMilliseconds();
            if (!millisecondsActivePerPlayer.ContainsKey(slotName))
            {
                millisecondsActivePerPlayer[slotName] = onlineTime;
            }
            else
            {
                millisecondsActivePerPlayer[slotName] += onlineTime;
            }
            isOnlinePerPlayer[slotName] = false;
        }

        public void Save()
        {
            // Set each player's original total checks
            foreach(var pair in checksPerPlayer)
            {
                Archipelago.Slots[pair.Key].TotalChecks = pair.Value;
            }

            // Keep track of players worth medals
            Slot? mostPerHour = null;
            Slot? leastPerHour = null;
            Slot? mostOnline = null;
            Slot? leastOnline = null;

            // Calculate Checks Per Hour & Time Online for each player
            foreach (var kvp in checksPerPlayer)
            {
                // Grab the player info
                var playerName = kvp.Key;
                if (!Archipelago.Slots.ContainsKey(playerName)) continue;
                var player = Archipelago.Slots[playerName];

                // Calculate the total number of checks found, MINUS the ones found from releases (this works because it runs after ReleaseTracker)
                int checksFoundFromOtherSources = (int)player.MethodOfChecksFound.Sum(x => x.Value);
                var totalChecksFound = kvp.Value;
                int checksFoundNormally = totalChecksFound - checksFoundFromOtherSources;

                // Stat: Store Total Checks and Checks found from other Sources
                player.MethodOfChecksFound.Add(new BasicStat("Found Normally", checksFoundNormally));
                player.TotalChecks = totalChecksFound;

                // Calculate the Checks Per Hour
                long millisecondsActive = millisecondsActivePerPlayer.ContainsKey(playerName) ? millisecondsActivePerPlayer[playerName] : 0;
                double hoursActive = millisecondsActive / (double)_hoursToMilliseconds;
                double checksPerHour = hoursActive > 0 ? (checksFoundNormally / hoursActive) : 0;

                // Set the player's Checks Per Hour and Active Time, then check for medal potential
                player.ChecksPerHour = checksPerHour;
                player.ActiveTimeOnline = TimeSpan.FromMilliseconds(millisecondsActive);

                // See if this player is eligible for any post-iteration medals
                if (mostPerHour == null || checksPerHour > mostPerHour?.ChecksPerHour) mostPerHour = player;
                if (leastPerHour == null || checksPerHour < leastPerHour?.ChecksPerHour) leastPerHour = player;
                if (mostOnline == null || mostOnline?.ActiveTimeOnline < player.ActiveTimeOnline) mostOnline = player;
                if (leastOnline == null || leastOnline?.ActiveTimeOnline > player.ActiveTimeOnline) leastOnline = player;

                // Medal: Efficient
                if (checksPerHour > 30.0)
                {
                    player.Medals.Add(new Medal("Efficient", "Maintained an Average of over 30 Checks per Hour", MudBlazor.Icons.Material.Filled.Speed));
                }

                // Medal: Thorough
                if (player.PercentageOfChecksBeforeRelease > 80.0f)
                {
                    player.Medals.Add(new Medal("Thorough Explorer", "Found over 80% of Checks before Releasing", MudBlazor.Icons.Material.Filled.TravelExplore));
                }

                // Medal: Low Percent of Checks Before Release
                if (player.PercentageOfChecksBeforeRelease < 50.0f)
                {
                    player.Medals.Add(new Medal("The Straightest Path is a Line", "Found less than 50% of Checks before Releasing", MudBlazor.Icons.Material.Filled.SportsScore));
                }
            }

            // Medal: Most Checks per Hour
            if(mostPerHour != null)
            {
                mostPerHour.Medals.Add(new Medal("Field Medic", "Achieved the Highest Checks per Hour Rate", MudBlazor.Icons.Material.Filled.MedicalServices));
            }

            // Medal: Least Checks per Hour
            if(leastPerHour != null)
            {
                leastPerHour.Medals.Add(new Medal("Slow and Steady", "Had the Lowest Checks per Hour Rate", MudBlazor.Icons.Material.Filled.PersonalInjury));
            }

            // Medal: Least Online
            if (leastOnline != null)
            {
                leastOnline.Medals.Add(new Medal("Visitor", "Spent the Least Amount of Time Online", MudBlazor.Icons.Material.Filled.Hail));
            }

            // Medal: Most Online
            if (mostOnline != null)
            {
                mostOnline.Medals.Add(new Medal("Dedicated", "Spent the Most Amount of Time Online", MudBlazor.Icons.Material.Filled.Kayaking));
            }
        }
    }
}
