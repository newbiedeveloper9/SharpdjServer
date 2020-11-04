using System;
using System.Collections.Generic;
using System.Text;
using SharpDj.Server.Management.HandlersAction;
using SharpDj.Server.Models;

namespace SharpDj.Server.Application.Dictionaries.Bags
{
    public class ActiveUserBag : IActionBag
    {
        public ServerUserModel ActiveUser { get; }

        public ActiveUserBag(ServerUserModel activeUser)
        {
            ActiveUser = activeUser;
        }
    }
}
