using Network;
using SharpDj.Common.Handlers.Dictionaries.Bags;

namespace SharpDj.Server.Application.Commands.Bags
{
    public class ConnectionBag : IActionBag
    {
        public Connection Connection { get; }

        public ConnectionBag(Connection connection)
        {
            Connection = connection;
        }
    }
}
