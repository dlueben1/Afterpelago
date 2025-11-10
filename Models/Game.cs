using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

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

        public string DirectoryName
        {
            get
            {
                return Regex.Replace(RealName.ToLower(), "[^a-zA-Z0-9]", "");
            }
        }

        public string OriginalSystem { get; set; } = "Unknown";

        public string CoverSource { get; set; } = string.Empty;
        public int? CoverCode { get; set; } = null;
        public string APTrackerSource { get; set; } = string.Empty;
        public Dictionary<string, Item> Items { get; set; } = new Dictionary<string, Item>();

        public Game(string name)
        {
            RealName = name;
        }

        #region Game Metadata

        /// <summary>
        /// Whether the game is Supported by Afterpelago by default
        /// </summary>
        public bool IsSupported { get; set; }

        public async Task DownloadBlobData()
        {
            // Build the blob path for our game
            var blobPath = Path.Combine(Program.BlobEndpoint, DirectoryName);

            // Grab the items.json file and process each supported item
            var itemsPath = Path.Combine(blobPath, "items.json");
            using(var httpClient = new HttpClient())
            {
                Console.WriteLine($"Downloading from {itemsPath}");
                var _items = await httpClient.GetFromJsonAsync<Item[]>(itemsPath) ?? [];
                for(int i = 0; i < _items.Length; i++)
                {
                    // Update the parent
                    _items[i].Parent = this;

                    // Add to our dictionary
                    if (!Items.ContainsKey(_items[i].Name)) Items.Add(_items[i].Name, _items[i]);

                    // If the word seems plural, create an alias that's singular
                    if (_items[i].Name.EndsWith('s'))
                    {
                        var singularName = _items[i].Name.TrimEnd('s');
                        if (!Items.ContainsKey(singularName)) Items.Add(singularName, _items[i]);
                    }
                }
            }
        }

        #endregion

        #region Comparison / Uniqueness

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

        #endregion
    }
}
