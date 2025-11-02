// Checks are, essentially, log entries indicating that a player has obtained an item or completed a task in the game, so we'll alias them.
global using Check = Afterpelago.Models.CheckObtainedLogEntry;

using Afterpelago.Models;

namespace Afterpelago
{
    public static class Archipelago
    {
        /// <summary>
        /// Represents whether the setup is complete or not
        /// </summary>
        public static bool SetupComplete { get; set; } = false;

        /// <summary>
        /// All unique games in this Archipelago session, indexed by their Real Name
        /// </summary>
        public static Dictionary<string, Game> Games { get; } = new Dictionary<string, Game>();

        /// <summary>
        /// All slots/players in this Archipelago session, indexed by their Player Name
        /// </summary>
        public static Dictionary<string, Slot> Slots { get; } = new Dictionary<string, Slot>();

        /// <summary>
        /// All checks in the game session, in the order they were obtained
        /// </summary>
        public static Check[] Checks { get; set; } = Array.Empty<Check>();
    }
}
