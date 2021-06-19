namespace SharpDj.Common.DTO
{
    public class PreviewRoomDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public int AmountOfPeople { get; set; }
        public int AmountOfAdministration { get; set; }

        public TrackDTO NextTrack { get; set; }
        public TrackDTO CurrentTrack { get; set; }
        public TrackDTO PreviousTrack { get; set; }
    }
}
