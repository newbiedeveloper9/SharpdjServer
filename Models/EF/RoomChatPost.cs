using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCPackets.Models;

namespace Server.Models
{
    public class RoomChatPost
    {
        [Key, Column("RoomChatPostId")]
        public int Id { get; set; }
        public string Text { get; set; }
        public string Color { get; set; }
        public User Author { get; set; }

        public RoomPostModel ToOutsideModel()
        {
            return new RoomPostModel()
            {
                Author = Author.ToUserClient(),
                Color = new ColorModel(Color),
                Message = Text,
                Id = Id
            };
        }
    }
}
