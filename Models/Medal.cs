namespace Afterpelago.Models
{
    public class Medal
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public Medal(string name, string desc, string icon)
        {
            Name = name;
            Description = desc;
            Icon = icon;
        }
    }
}
