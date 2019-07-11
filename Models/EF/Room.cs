using Newtonsoft.Json;
using SCPackets.CreateRoom.Container;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Server.Models.EF;

namespace Server.Models
{
    public class Room
    {
        [Key, Column("RoomId")]
        public int Id { get; set; }
        public string Name { get; set; }
        public User Author { get; set; }
        public string ImagePath { get; set; }
        public RoomConfig RoomConfig { get; set; }
        public ICollection<MediaHistory> Media { get; set; }
        public ICollection<RoomChatPost> Posts { get; set; }
        public ICollection<UserClaim> UserClaims { get; set; }
        public ICollection<UserClaim> RoleClaims { get; set; }

        public void ImportByRoomModel(RoomModel model, User author)
        {
            Id = model.Id;
            Name = model.Name;
            Author = author;
            ImagePath = model.ImageUrl;
            RoomConfig = new RoomConfig()
            {
                ChatType = ChatType.All,
                LocalEnterMessage = model.LocalEnterMessage,
                LocalLeaveMessage = model.LocalLeaveMessage,
                PublicEnterMessage = model.PublicEnterMessage,
                PublicLeaveMessage = model.PublicLeaveMessage,
            };
        }

        public RoomModel ToRoomModel()
        {
            return new RoomModel()
            {
                Id = Id,
                ImageUrl = ImagePath,
                Name = Name,
                LocalEnterMessage = RoomConfig.LocalEnterMessage,
                LocalLeaveMessage = RoomConfig.LocalLeaveMessage,
                PublicEnterMessage = RoomConfig.PublicEnterMessage,
                PublicLeaveMessage = RoomConfig.PublicLeaveMessage,
            };
        }

        public RoomInstance ToRoomInstance()
        {
            var serializedParent = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<RoomInstance>(serializedParent);
        }
    }
}