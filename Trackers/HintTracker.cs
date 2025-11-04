using Afterpelago.Models;
using Afterpelago.Models.Statistics;
using Afterpelago.Utilities;

namespace Afterpelago.Trackers
{
    public class HintTracker : ITracker
    {
        private HintStats _stats;
        private bool _firstHintFound = false;

        private Dictionary<string, DateTime> _pendingHints;

        public HintTracker()
        {
            _stats = new HintStats
            {
                HintsByPlayer = new Dictionary<string, int>(),
                HintsReferencingPlayer = new Dictionary<string, int>(),
                TimeToFulfillHints = new Dictionary<string, TimeSpan>(),
                TotalHintCount = 0
            };

            _pendingHints = new Dictionary<string, DateTime>();
        }

        public void ParseLine(LogEntry entry)
        {
            // If the entry is null, back out
            if (entry == null) return;

            // If this is a check, and not a hint, we'll see if we're waiting for a payoff on it
            if(entry.Category == LogEntryType.CheckFound) CheckForHintPayoff((Check)entry);

            // Ignore non-hints moving forward
            if (entry.Category != LogEntryType.Hint)
            {
                return;
            }

            // Add to total hint count
            _stats.TotalHintCount++;

            // Get hint entry data
            var hintEntry = entry as HintLogEntry;
            if (hintEntry == null) return;

            // Keep track of this hint to see when it gets paid off
            if (!_pendingHints.ContainsKey(hintEntry.UniqueId)) Console.WriteLine($"ADDING PENDING HINT: {hintEntry.UniqueId}");
            if (!_pendingHints.ContainsKey(hintEntry.UniqueId)) _pendingHints.Add(hintEntry.UniqueId, hintEntry.Timestamp);

            // If this is the first hint, store it
            if (!_firstHintFound) _stats.FirstHint = hintEntry;

            // Increment needy player hint count
            if (_stats.HintsByPlayer.ContainsKey(hintEntry.ReceiverName))
            {
                _stats.HintsByPlayer[hintEntry.ReceiverName]++;
            }
            else
            {
                _stats.HintsByPlayer[hintEntry.ReceiverName] = 1;
            }

            // Increment blocking player hint count
            if (_stats.HintsReferencingPlayer.ContainsKey(hintEntry.SenderName))
            {
                _stats.HintsReferencingPlayer[hintEntry.SenderName]++;
            }
            else
            {
                _stats.HintsReferencingPlayer[hintEntry.SenderName] = 1;
            }
        }

        private void CheckForHintPayoff(Check check)
        {
            // See if this check was hinted at
            if (_pendingHints.ContainsKey(check.UniqueId))
            {
                // Calculate time to find
                var timeToFind = check.Timestamp - _pendingHints[check.UniqueId];

                // Store it for later
                _stats.TimeToFulfillHints[check.UniqueId] = timeToFind;
            }
        }

        public void Save()
        {
            // Determine which player had the most hints
            var _mostHintsPlayer = _stats.HintsByPlayer.OrderByDescending(kvp => kvp.Value).FirstOrDefault();
            _stats.PlayerWithMostHints = _stats.TotalHintCount == 0 ? "None (No Hints Used/Found)" : $"{_mostHintsPlayer.Key} ({_mostHintsPlayer.Value} Hints Obtained)";

            // Determine which player had the hints to fulfill
            var _mostRefPlayer = _stats.HintsReferencingPlayer.OrderByDescending(kvp => kvp.Value).FirstOrDefault();
            _stats.PlayerMostReferencedInHints = _stats.TotalHintCount == 0 ? "None (No Hints Used/Found)" : $"{_mostRefPlayer.Key} ({_mostRefPlayer.Value} Hints were for this Player's World)";

            // Determine which hint had the longest and shortest fulfillment times (these can eventually be shortened to a single pass, in the future, once the basics are done)
            var _shortestPayoff = _stats.TimeToFulfillHints.OrderBy(kvp => kvp.Value).FirstOrDefault();
            var _shortestPayoffHint = Archipelago.Hints.FirstOrDefault(hint => hint.UniqueId == _shortestPayoff.Key);
            var _longestPayoff = _stats.TimeToFulfillHints.OrderByDescending(kvp => kvp.Value).FirstOrDefault();
            var _longestPayoffHint = Archipelago.Hints.FirstOrDefault(hint => hint.UniqueId == _longestPayoff.Key);
            _stats.FastestPayoff = _shortestPayoffHint;
            _stats.LongestPayoff = _longestPayoffHint;

            // Update the Hint Statistics
            Statistics.Hints = _stats;

            // At this point, back out if we have no hints
            if (_stats.TotalHintCount == 0) return;

            // Medal: First Hint Found
            if (_stats.FirstHint != null)
            {
                var firstHintSlot = Archipelago.Slots.GetValueOrDefault(_stats.FirstHint.ReceiverName);
                if (firstHintSlot != null)
                {
                    firstHintSlot.Medals.Add(new Medal("Very Curious", "Was the First Player to Obtain a Hint", MudBlazor.Icons.Material.Filled.Lightbulb));
                }
            }

            // Medal: Most Hints Obtained
            var playerWithMostHints = Archipelago.Slots.GetValueOrDefault(_mostHintsPlayer.Key);
            if (playerWithMostHints != null)
            {
                playerWithMostHints.Medals.Add(new Medal("Fact Finder", "Found the Most Hints for their Items", MudBlazor.Icons.Material.Filled.Signpost));
            }

            // Medal: Keeper of Secrets
            var keeperOfSecrets = Archipelago.Slots.GetValueOrDefault(_mostRefPlayer.Key);
            if (keeperOfSecrets != null)
            {
                keeperOfSecrets.Medals.Add(new Medal("Keeper of Secrets", "Their World had the most Items referenced in Hints", MudBlazor.Icons.Material.Filled.GpsFixed));
            }
        }
    }
}
