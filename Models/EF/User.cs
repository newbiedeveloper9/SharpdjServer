using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SCPackets;
using SCPackets.Models;
using Rank = SCPackets.Models.Rank;

namespace Server.Models
{
    public class User
    {
        [Key, Column("UserId")]
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

        public UserClientModel ToUserClient()
        {
            return new UserClientModel(this.Id, this.Username, this.Rank);
        }
    }
}
