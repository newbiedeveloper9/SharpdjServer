using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Factory
{
    public interface IChatMessageFactory
    {
        RoomChatMessageEntity GetChatMessage(UserEntity user, byte[] rgb, string text);
    }
}