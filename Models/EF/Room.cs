using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using SCPackets.CreateRoom.Container;

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

        public Room ToServerModel(RoomModel model, User author)
        {
            return new Room()
            {
                Name = model.Name,
                Author = author,
                ImagePath = model.ImageUrl,
                RoomConfig = new RoomConfig()
                {
                    ChatType = ChatType.All,
                    LocalEnterMessage = model.LocalEnterMessage,
                    LocalLeaveMessage =  model.LocalLeaveMessage,
                    PublicEnterMessage = model.PublicEnterMessage,
                    PublicLeaveMessage =  model.PublicLeaveMessage,
                },
                Media = new List<MediaHistory>()
                {
                    
                },
                Posts = new List<RoomChatPost>()
                {

                }
            };
        }

        public RoomInstance ToRoomInstance()
        {
            var serializedParent = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<RoomInstance>(serializedParent);
        }
    }
}