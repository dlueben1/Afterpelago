using System.Runtime.Serialization;

namespace Afterpelago.Models
{
    [DataContract]
    public class Slot
    {
        /// <summary>
        /// The name of the Player/Slot
        /// </summary>
        [DataMember]
        public string PlayerName { get; private set; }

        /// <summary>
        /// The game that this slot played
        /// </summary>
        [DataMember]
        public Game Game { get; private set; }

        [DataMember]
        public int? FinishOrder { get; set; } = null;

        [IgnoreDataMember]
        public List<Medal> Medals { get; private set; }

        public Slot(string name, Game game)
        {
            PlayerName = name;
            Game = game;
            Medals = new List<Medal>();
        }
    }
        
}
