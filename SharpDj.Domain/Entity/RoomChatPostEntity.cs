using System.ComponentModel.DataAnnotations;

namespace SharpDj.Domain.Entity
{
    public class RoomChatPostEntity
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public byte[] Color { get; set; }
        public UserEntity Author { get; set; }
    }
}
