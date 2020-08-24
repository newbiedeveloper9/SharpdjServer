namespace SharpDj.Domain.Entity
{
    public class MediaHistory
    {
        public int Id { get; set; }
        public MediaType MediaType { get; set; }
        public string Url { get; set; }
    }

    public enum MediaType
    {
        SoundCloud,
        Youtube
    }
}
