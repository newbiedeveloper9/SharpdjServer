using System.Collections.Generic;
using SharpDj.Common.DTO;

namespace SharpDj.Domain.Entity
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public User Author { get; set; }
        public string ImagePath { get; set; }
        public RoomConfig Config { get; set; }
        public ICollection<MediaHistory> Media { get; set; }
        public ICollection<RoomChatPost> Posts { get; set; }
        public ICollection<UserClaim> UserClaims { get; set; }
        public ICollection<UserClaim> RoleClaims { get; set; }
    }
}