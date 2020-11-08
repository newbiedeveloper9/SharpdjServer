using SharpDj.Server.Application.Models;

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
