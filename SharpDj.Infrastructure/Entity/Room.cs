using System.Collections.Generic;
using Newtonsoft.Json;
using SCPackets.Dto;
using SCPackets.Models;
using SharpDj.Server.Models;

namespace SharpDj.Server.Entity
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

        public void ImportByRoomModel(RoomDetailsDto model, User author)
        {
            Id = model.Id;
            Name = model.Name;
            Author = author;
            ImagePath = model.ImageUrl;
            Config = new RoomConfig()
            {
                ChatType = ChatType.All,
                LocalEnterMessage = model.LocalEnterMessage,
                LocalLeaveMessage = model.LocalLeaveMessage,
                PublicEnterMessage = model.PublicEnterMessage,
                PublicLeaveMessage = model.PublicLeaveMessage,
            };
        }

        public RoomDetailsDto ToRoomModel()
        {
            return new RoomDetailsDto()
            {
                Id = Id,
                ImageUrl = ImagePath,
                Name = Name,
                LocalEnterMessage = Config.LocalEnterMessage,
                LocalLeaveMessage = Config.LocalLeaveMessage,
                PublicEnterMessage = Config.PublicEnterMessage,
                PublicLeaveMessage = Config.PublicLeaveMessage,
            };
        }

        public RoomInstance ToRoomInstance()
        {
            var serializedParent = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<RoomInstance>(serializedParent);
        }
    }
}