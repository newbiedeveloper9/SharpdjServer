using System;
using System.Collections.Generic;
using System.Text;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Factory
{
    public class ChatMessageFactory : IChatMessageFactory
    {
        public RoomChatMessageEntity GetChatMessage(UserEntity user, byte[] rgb, string text)
        {
            return new RoomChatMessageEntity()
            {
                User = user,
                Color = rgb,
                Text = text
            };
        }
    }
}
