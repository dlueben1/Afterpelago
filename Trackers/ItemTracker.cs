using Afterpelago.Models;

namespace Afterpelago.Trackers
{
    public class ItemTracker : ITracker
    {
        private Dictionary<string, string> firstItems = new Dictionary<string, string>();
        private Dictionary<string, Check> firstItem_Checks = new Dictionary<string, Check>();

        public void ParseLine(LogEntry entry)
        {
            // Is this a check?
            if (entry.Category != LogEntryType.CheckFound) return;
            var check = entry as Check;

            // Get the Item and Recipient
            var recipient = check.ReceiverName;
            if(!firstItems.ContainsKey(recipient))
            {
                firstItems[recipient] = check.ItemName;
                firstItem_Checks[recipient] = check;
                return;
            }
        }

        public void Save()
        {
            foreach(var kvp in firstItems)
            {
                // If the player doesn't exist, back out
                if (!Archipelago.Slots.ContainsKey(kvp.Key)) continue;

                // Grab the player
                var player = Archipelago.Slots[kvp.Key];

                // Set the first item received
                Item? firstItem = null;
                if (player.Game.IsSupported && player.Game.Items != null)
                {
                    firstItem = player.Game.Items.ContainsKey(kvp.Value) ? player.Game.Items[kvp.Value] : null;
                }
                player.FirstItemReceived = firstItem ?? new Item { Name = kvp.Value, Type = "Unknown", Img = string.Empty };
                player.FirstItemLogEntry = firstItem_Checks[player.PlayerName];
            }
        }
    }
}
