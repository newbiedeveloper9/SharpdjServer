﻿using SCPackets.Models;
using SharpDj.Domain.Entity;

namespace SharpDj.Domain.Mapper
{
    public class ChatMessageMapper : IChatMessageMapper
    {
        public ChatMessage MapToDto(RoomChatMessageEntity entity)
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
                    Id = dto.Author.Id,
                    Username = dto.Author.Username,
                }
            };
        }
    }
}
