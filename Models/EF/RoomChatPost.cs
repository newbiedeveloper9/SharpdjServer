﻿using SCPackets.Models;

namespace Server.Models
{
    public class RoomChatPost
    {
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
