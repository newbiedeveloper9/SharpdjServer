using System.Collections.Generic;
using SharpDj.Common.DTO;

namespace SharpDj.Domain.Entity
{
    public class RoomEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }

        public UserEntity Author { get; set; }
        public long AuthorId { get; set; }

        public RoomConfigEntity Config { get; set; }
        public ICollection<RoomChatMessageEntity> Posts { get; set; }
    }
}