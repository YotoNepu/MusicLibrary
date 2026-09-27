using System.Xml.Serialization;

namespace MusicLibrary.Core
{
    [XmlInclude(typeof(Track))]
    public abstract class MusicItem : IRateable
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public int Year { get; set; }

        public double Rating { get; set; }

        public abstract string Type { get; }

        public override string ToString()
        {
            return Artist + " - " + Title;
        }
    }
}