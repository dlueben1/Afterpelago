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

        public Item FirstItemReceived { get; set; }
        public Check FirstItemLogEntry { get; set; }

        public TimeSpan ActiveTimeOnline { get; set; } = TimeSpan.Zero;

        public double ChecksPerHour { get; set; } = 0;

        public float PercentageOfChecksBeforeRelease
        {
            get
            {
                int numberOfChecksBeforeRelease = TotalChecksFound  - EstimatedChecksFromRelease;
                return ((float)numberOfChecksBeforeRelease / (float)TotalChecksFound) * 100f;
            }
        }

        public int TotalChecksFound { get; set; } = 0;

        /// <summary>
        /// @todo Eventually this may want to exist as a .Count or .Length of a collection of checks
        /// </summary>
        public int EstimatedChecksFromRelease { get; set; } = 0;

        public Slot(string name, Game game)
        {
            PlayerName = name;
            Game = game;
            Medals = new List<Medal>();
        }
    }
        
}
