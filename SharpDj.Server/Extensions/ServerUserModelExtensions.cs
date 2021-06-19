using System.Collections.Generic;
using Network;
using SharpDj.Server.Application.Models;

namespace SharpDj.Server.Extensions
{
    public static class ServerUserModelExtensions
    {
        public static List<Connection> GetAllConnections(this IEnumerable<ServerUserModel> users)
        {
            var connections = new List<Connection>();
            foreach (var user in users)
            {
                connections.AddRange(user.Connections);
            }

            return connections;
        }
    }
}