using Network;
using Server.Models;

namespace Server.Management
{
    public class ServerUserModel
    {
        public ServerUserModel(User user, Connection connection)
        {
            User = user;
            Connection = connection;
        }

        public User User { get; set; }
        public Connection Connection { get; set; }
    }
}
