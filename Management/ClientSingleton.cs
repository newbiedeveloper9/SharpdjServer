using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Management
{
    public sealed class ClientSingleton
    {
        private static readonly Lazy<ClientSingleton> lazy =
            new Lazy<ClientSingleton>(() => new ClientSingleton());

        public static ClientSingleton Instance => lazy.Value;

        private ClientSingleton()
        {
            Users = new List<User>();
        }

        public List<User> Users { get; set; }
    }
}
