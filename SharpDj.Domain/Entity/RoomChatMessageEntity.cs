
namespace SharpDj.Domain.Entity
{
    public class RoomChatMessageEntity
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public byte[] Color { get; set; } //TODO ASAP

        public UserEntity User { get; set; }
        public long UserId { get; set; }

        public RoomEntity Room { get; set; }
        public int RoomId { get; set; }
    }
}
