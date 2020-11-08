using Network;

namespace SharpDj.Server.Application.Dictionaries.Bags
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
