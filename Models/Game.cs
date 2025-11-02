using System.Runtime.Serialization;

namespace Afterpelago.Models
{
    [DataContract]
    public class Game
    {
        /// <summary>
        /// The name of the Game, as it shows up in the log file exactly
        /// </summary>
        [DataMember]
        public string RealName { get; private set; }

        /// <summary>
        /// The name of the Game to display to the user. In some cases, this may be different from the Real Name.
        /// </summary>
        [IgnoreDataMember]
        public string FriendlyName
        {
            get
            {
                return string.IsNullOrEmpty(friendlyName) ? RealName : friendlyName;
            }
            set
            {
                friendlyName = value;
            }
        }
        private string friendlyName = string.Empty;

        public Game(string name)
        {
            RealName = name;
        }

        /// <summary>
        /// We can assume, for the sake of simplicity, that what makes a game unique is it's real name.
        /// Games like "Zelda: Ocarina of Time" and "Ship of Harkinian" would be considered different games.
        /// 
        /// The log file doesn't report the "Game ID" of the game, so we can't use that for uniqueness.
        /// </summary>
        /// <returns>The Hash Code needed to determine this object's uniqueness</returns>
        public override int GetHashCode()
        {
            return RealName.GetHashCode();
        }

        /// <summary>
        /// We can assume, for the sake of simplicity, that what makes a game unique is it's real name.
        /// Games like "Zelda: Ocarina of Time" and "Ship of Harkinian" would be considered different games.
        /// 
        /// The log file doesn't report the "Game ID" of the game, so we can't use that for uniqueness.
        /// </summary>
        /// <returns>Whether this Game is equal to another Game, or not</returns>
        public override bool Equals(object? obj)
        {
            if (obj is Game otherGame)
            {
                return this.RealName == otherGame.RealName;
            }
            return false;
        }
    }
}
