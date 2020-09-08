using System;
using System.Collections.Generic;
using System.Text;
using SCPackets.Models;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Mapper
{
    public class ChatMessageMapperService : IDualMapper<RoomChatPostEntity, ChatMessage>
    {
        public ChatMessage MapToDTO(RoomChatPostEntity entity)
        {
            return new ChatMessage()
            {
                Author = entity.Author.ToUserClient(),
                Color = new Color(entity.Color),
                Message = entity.Text,
                Id = entity.Id
            };
        }

        public RoomChatPostEntity MapToEntity(ChatMessage dto)
        {
            return new RoomChatPostEntity()
            {
                Id = dto.Id,
                Color = dto.Color.Hex,
                Author = new UserEntity()
                {
                    Id = (int)dto.Author.Id,
                    Username = dto.Author.Username,
                    Rank = dto.Author.Rank
                }
            };
        }
    }
}
