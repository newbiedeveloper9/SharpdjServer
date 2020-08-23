using SCPackets.Models;

namespace SharpDj.Server.Entity
{
    public class RoomChatPost
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public User Author { get; set; }

        public ChatMessage ToOutsideModel()
        {
            return new ChatMessage()
            {
                Author = Author.ToUserClient(),
                Color = new Color(Color),
                Message = Text,
                Id = Id
            };
        }
    }
}
