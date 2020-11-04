using System.ComponentModel.DataAnnotations;

namespace SharpDj.Domain.Entity
{
    public class RoomChatMessageEntity
    {
        public ulong Id { get; set; }
        public string Text { get; set; }
        public byte[] Color { get; set; } //TODO ASAP

        public virtual UserEntity User { get; set; }
        public ulong UserId { get; set; }
    }
}
