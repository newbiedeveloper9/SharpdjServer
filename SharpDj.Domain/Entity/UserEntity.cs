using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SCPackets.Models;
using SharpDj.Common.Enums;

namespace SharpDj.Domain.Entity
{
    public class UserEntity
    {
        public ulong Id { get; set; }
        public string Username { get; set; }
        public Rank Rank { get; set; }
        public string Email { get; set; }
        public UserAuthEntity UserAuthEntity { get; set; }
        public ICollection<UserEntity> Friends { get; set; }
        public string AvatarUrl { get; set; } = "";

        public override string ToString()
        {
            return $"[{Id}][\"{UserAuthEntity?.Login}\"] Username: {Username}, Rank: {Rank}, Email: {Email}";
        }

        public UserClient ToUserClient()
        {
            return new UserClient(this.Id, this.Username, this.Rank);
        }
    }
}
