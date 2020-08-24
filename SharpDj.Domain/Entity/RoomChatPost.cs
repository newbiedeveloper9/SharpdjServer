namespace SharpDj.Domain.Entity
{
    public class RoomChatPost
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public User Author { get; set; }
    }
}
