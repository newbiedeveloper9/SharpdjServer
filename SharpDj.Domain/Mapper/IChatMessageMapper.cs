using SCPackets.Models;
using SharpDj.Domain.Entity;
using SharpDj.Domain.Interfaces;

namespace SharpDj.Domain.Mapper
{
    public interface IChatMessageMapper : IDualMapper<RoomChatMessageEntity, ChatMessage>
    {
        
    }
}