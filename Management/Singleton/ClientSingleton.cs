using System;
using System.Collections.Generic;

namespace Server.Management.Singleton
{
    public sealed class ClientSingleton
    {
        private static readonly Lazy<ClientSingleton> lazy =
            new Lazy<ClientSingleton>(() => new ClientSingleton());

        public static ClientSingleton Instance => lazy.Value;

        private ClientSingleton()
        {
            Users = new List<ServerUserModel>();
        }

        public List<ServerUserModel> Users { get; set; }
    }
}
