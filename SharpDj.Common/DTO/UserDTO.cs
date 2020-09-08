using System;
using System.Collections.Generic;
using System.Text;
using SharpDj.Common.Enums;

namespace SharpDj.Common.DTO
{
    public class UserDTO
    {
        public long Id { get; private set; }
        public string Username { get; private set; }
        public Rank Rank { get; private set; }
        public string Avatar { get; private set; }
    }
}
