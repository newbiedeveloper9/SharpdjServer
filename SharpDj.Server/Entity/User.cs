using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SCPackets.Models;
using Rank = SCPackets.Enums.Rank;

namespace SharpDj.Server.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public Rank Rank { get; set; }

        public UserAuth UserAuth { get; set; }

        public List<User> FriendList { get; set; }

        public string AvatarUrl { get; set; } = "";

        public override string ToString()
        {
            return $"[{Id}][\"{UserAuth?.Login}\"] Username: {Username}, Rank: {Rank}, Email: {Email}";
        }

        public UserClient ToUserClient()
        {
            return new UserClient(this.Id, this.Username, this.Rank);
        }
    }
}
