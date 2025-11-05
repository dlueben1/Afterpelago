namespace Afterpelago.Models
{
    public class Item
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Img { get; set; }
        public Game Parent { get; set; }
        public string? ImageEndpoint
        {
            get
            {
                if (string.IsNullOrEmpty(Img)) return null;
                var imageSuffix = Img;
                imageSuffix = imageSuffix.Replace("\\", "/");
                if (imageSuffix.StartsWith("/")) imageSuffix = imageSuffix.Substring(1);
                return Path.Combine(Program.BlobEndpoint, Parent.DirectoryName, imageSuffix);
            }
        }
    }
}
