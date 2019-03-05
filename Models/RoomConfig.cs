using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class RoomConfig
    {
        [Key, Column("RoomConfigId")]
        public int Id { get; set; }

        public ChatType ChatType { get; set; } = ChatType.All;
    }

    public enum ChatType
    {
        StaffOnly = 3,
        DjsOnly = 2,
        SubscribersOnly = 1,
        All = 0,
    }
}