using SharpDj.Server.Management.HandlersAction;
using System;
using System.Collections.Generic;
using System.Text;
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
