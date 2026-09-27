namespace MusicLibrary.Core
{
    public class Track : MusicItem
    {
        public string Album { get; set; }

        public string Genre { get; set; }

        public int DurationSeconds { get; set; }

        public override string Type
        {
            get { return "Track"; }
        }
    }
}