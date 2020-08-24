using System;
using System.Collections.Generic;
using System.Text;
using SCPackets.Models;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Mapper
{
    public class ChatMessageMapperService : IDualMapper<RoomChatPost, ChatMessage>
    {
        public ChatMessage MapToDTO(RoomChatPost entity)
        {
            return new ChatMessage()
            {
                Author = entity.Author.ToUserClient(),
                Color = new Color(entity.Color),
                Message = entity.Text,
                Id = entity.Id
            };
        }

        public RoomChatPost MapToEntity(ChatMessage dto)
        {
            return new RoomChatPost()
            {
                Id = dto.Id,
                Color = dto.Color.Hex,
                Author = new User()
                {
                    Id = (int)dto.Author.Id,
                    Username = dto.Author.Username,
                    Rank = dto.Author.Rank
                }
            };
        }
    }
}
