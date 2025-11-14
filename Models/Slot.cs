using System.Runtime.Serialization;

namespace Afterpelago.Models
{
    [DataContract]
    public class Slot
    {
        #region Slot Attributes

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

        #endregion

        #region Check-Related Statistics

        /// <summary>
        /// The total number of checks from this Slot's world
        /// </summary>
        public int TotalChecks { get; set; } = 0;

        /// <summary>
        /// A collection to hold the count of checks found by their method
        /// (Collected by the Player, Released on Clear, Released by another Player's Clear)
        /// </summary>
        public List<BasicStat> MethodOfChecksFound { get; private set; }

        /// <summary>
        /// The number of your items that were supposed to be found in other player's worlds, but
        /// you obtained by clearing your goal.
        /// </summary>
        public int OtherPeoplesChecksFoundByMyRelease { get; set; }

        /// <summary>
        /// The number of Checks-Per-Hour obtained by this slot.
        /// Excludes checks found by yours or other peoples' releases
        /// </summary>
        public double ChecksPerHour { get; set; } = 0;

        /// <summary>
        /// The number of Checks found normally, without releases.
        /// </summary>
        public decimal NumberOfChecksFoundNormally
        {
            get
            {
                return MethodOfChecksFound.First(x => x.Label == "Found Normally").Value;
            }
        }

        /// <summary>
        /// The number of Checks found by this slot's release (clearing the game)
        /// </summary>
        public decimal NumberOfChecksFoundByMyRelease
        {
            get
            {
                return MethodOfChecksFound.First(x => x.Label == "Found by Clearing").Value;
            }
        }

        public decimal NumberOfChecksFoundByOtherReleases
        {
            get
            {
                return MethodOfChecksFound.First(x => x.Label == "Did Not Find Themselves").Value;
            }
        }

        /// <summary>
        /// The Percentage of Checks found normally before clearing the game.
        /// Excludes checks found by yours or other peoples' releases
        /// </summary>
        public float PercentageOfChecksBeforeRelease
        {
            get
            {
                return ((float)NumberOfChecksFoundNormally / TotalChecks) * 100f;
            }
        }

        #endregion

        [DataMember]
        public int? FinishOrder { get; set; } = null;

        [IgnoreDataMember]
        public List<Medal> Medals { get; private set; }

        public Item FirstItemReceived { get; set; }
        public Check FirstItemLogEntry { get; set; }

        public TimeSpan ActiveTimeOnline { get; set; } = TimeSpan.Zero;

        public Slot(string name, Game game)
        {
            PlayerName = name;
            Game = game;

            // Initialize collections
            Medals = new();
            MethodOfChecksFound = new();
        }
    }
        
}
