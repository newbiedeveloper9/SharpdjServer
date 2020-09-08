namespace SharpDj.Domain.Entity
{
    public class RoomChatPostEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public UserEntity Author { get; set; }
    }
}
