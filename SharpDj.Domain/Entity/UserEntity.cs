using System.Collections.Generic;
using SCPackets.Models;

namespace SharpDj.Domain.Entity
{
    public class UserEntity
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public UserAuthEntity UserAuthEntity { get; set; }

        public ICollection<RoomEntity> Rooms { get; set; }
        public ICollection<UserAuditEntity> UserAudits {get; set; }
        public ICollection<RoomChatMessageEntity> Posts { get; set; }

        public override string ToString()
        {
            return $"[{Id}][\"{UserAuthEntity?.Login}\"] Username: {Username}, Email: {Email}";
        }

        public UserClient ToUserClient()
        {
            return new UserClient(this.Id, this.Username);
        }
    }
}
