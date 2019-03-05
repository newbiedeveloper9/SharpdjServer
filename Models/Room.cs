using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Room
    {
        [Key, Column("RoomId")]
        public int Id { get; set; }
        public int Name { get; set; }
        public User Author { get; set; }
        public string ImagePath { get; set; }
        public RoomConfig RoomConfig { get; set; }
    }
}