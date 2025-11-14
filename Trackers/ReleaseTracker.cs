using Afterpelago.Models;
using Afterpelago.Models.Statistics;

namespace Afterpelago.Trackers
{
    public class ReleaseTracker : ITracker
    {
        private List<ReleaseLogEntry> Releases { get; set; } = new();
        public void ParseLine(LogEntry entry)
        {
            // Ignore non-release entries
            if (entry.Category != LogEntryType.Release) return;

            // Log release entry
            Releases.Add((ReleaseLogEntry)entry);
        }

        public void Save()
        {
            // Store all releases
            Archipelago.Releases = Releases.DistinctBy(x => x.SlotName).ToArray();

            // Update all Players with their release statistics
            for (int i = 0; i < Archipelago.Releases.Length; i++)
            {
                // Get the release data
                var release = Archipelago.Releases[i];

                // Ignore release if somehow the slot is null
                if (release.Slot == null) continue;
                
                // Mark the finish order for this Slot
                release.Slot.FinishOrder = i + 1;

                // Get the range of time that will count as release (eventually it'd be better to walk the distance between checks maybe)
                var releaseTimeStart = release.Timestamp.AddSeconds(-5);
                var releaseTimeEnd = release.Timestamp.AddSeconds(5);

                // Mark all related checks from this release
                var releasedChecks = Archipelago.Checks.Where(c => 
                    (c.SenderName == release.SlotName || c.ReceiverName == release.SlotName) && 
                    c.Timestamp >= releaseTimeStart && 
                    c.Timestamp <= releaseTimeEnd
                ).ToArray();
                for(int c = 0; c < releasedChecks.Length; c++)
                {
                    var check = releasedChecks[c];
                    check.Trigger = CheckTrigger.Release;
                    check.TriggerSlotName = release.SlotName;
                }

                // Mark the ones from this player's world
                var fromReleasedChecks = releasedChecks.Where(c => c.SenderName == release.SlotName).ToArray();

                // Stats: Track the number of our items from other people's worlds that were found by releasing
                release.Slot.OtherPeoplesChecksFoundByMyRelease = releasedChecks.Length - fromReleasedChecks.Length;

                // Stats: Add Number of Checks found by this slot's release
                release.Slot.MethodOfChecksFound.Add(new BasicStat("Found by Clearing", fromReleasedChecks.Length));

                // Medal: First Finish
                if (i == 0 && release.Slot != null)
                {
                    release.Slot.Medals.Add(new Medal("First Place", "Was the First Player to Release their Items", MudBlazor.Icons.Material.Filled.Flag));
                }

                // Medal: Second Finish
                else if (i == 1 && release.Slot != null)
                {
                    release.Slot.Medals.Add(new Medal("Second Place", "Was the Second Player to Release their Items", MudBlazor.Icons.Material.Filled.Flag));
                }

                // Medal: Third Finish
                else if (i == 2 && release.Slot != null)
                {
                    release.Slot.Medals.Add(new Medal("Third Place", "Was the Third Player to Release their Items", MudBlazor.Icons.Material.Filled.Flag));
                }
            }

            // Once all Releases and their Checks have been processed, find how many checks were found by others
            foreach(var pair in Archipelago.Slots)
            {
                // Grab the Slot
                var slot = pair.Value;

                // Stats: Add Number of Checks found by other people's clears
                var checksFoundByOthers = Archipelago.Checks.Where(c => 
                    c.SenderName == slot.PlayerName && 
                    c.Trigger == CheckTrigger.Release && 
                    c.TriggerSlotName != slot.PlayerName
                ).ToArray();
                slot.MethodOfChecksFound.Add(new BasicStat("Did Not Find Themselves", checksFoundByOthers.Length));
            }
        }
    }
}
