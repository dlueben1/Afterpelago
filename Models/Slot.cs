namespace Afterpelago.Models
{
    public class Slot
    {
        /// <summary>
        /// The name of the Player/Slot
        /// </summary>
        public string PlayerName { get; private set; }

        /// <summary>
        /// The game that this slot played
        /// </summary>
        public Game Game { get; private set; }

        public Slot(string name, Game game)
        {
            PlayerName = name;
            Game = game;
        }
    }
        
}
