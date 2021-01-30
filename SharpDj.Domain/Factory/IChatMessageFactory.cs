using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Factory
{
    public interface IChatMessageFactory
    {
        RoomChatMessageEntity CreateChatMessage(UserEntity user, byte[] rgb, string text);
    }
}