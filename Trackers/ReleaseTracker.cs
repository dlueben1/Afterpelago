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
                var release = Archipelago.Releases[i];
                Console.WriteLine($"RELEASE SLOT: {release.SlotName}");
                if (release.Slot != null) release.Slot.FinishOrder = i + 1;

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
        }
    }
}
