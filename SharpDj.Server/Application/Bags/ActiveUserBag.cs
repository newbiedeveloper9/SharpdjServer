using SharpDj.Common.Handlers.Dictionaries.Bags;
using SharpDj.Server.Application.Models;

namespace SharpDj.Server.Application.Bags
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
