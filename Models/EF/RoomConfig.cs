using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class RoomConfig
    {
        public int Id { get; set; }

        public ChatType ChatType { get; set; } = ChatType.All;

        public string PublicEnterMessage { get; set; }
        public string PublicLeaveMessage { get; set; }
        public string LocalEnterMessage { get; set; }
        public string LocalLeaveMessage { get; set; }
    }

    public enum ChatType
    {
        StaffOnly = 3,
        DjsOnly = 2,
        SubscribersOnly = 1,
        All = 0,
    }
}