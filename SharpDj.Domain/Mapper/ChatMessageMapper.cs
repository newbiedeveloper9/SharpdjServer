using System;
using System.Collections.Generic;
using System.Text;
using SCPackets.Models;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Mapper
{
    public class ChatMessageMapper : IDualMapper<RoomChatMessageEntity, ChatMessage>
    {
        public ChatMessage MapToDTO(RoomChatMessageEntity entity)
        {
            return new ChatMessage()
            {
                Author = entity.User.ToUserClient(),
                Color = new Color().SetColor(entity.Color),
                Message = entity.Text,
                Id = entity.Id
            };
        }

        public RoomChatMessageEntity MapToEntity(ChatMessage dto)
        {
            return new RoomChatMessageEntity()
            {
                Id = dto.Id,
                Color = dto.Color.RGB,
                User = new UserEntity()
                {
                    Id = (int)dto.Author.Id,
                    Username = dto.Author.Username,
                    Rank = dto.Author.Rank
                }
            };
        }
    }
}
