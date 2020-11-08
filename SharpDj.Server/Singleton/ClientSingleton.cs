using System;
using SCPackets;
using SharpDj.Common;
using SharpDj.Common.ListWrapper;
using SharpDj.Server.Application.Models;

namespace SharpDj.Server.Singleton
{
    public sealed class ClientSingleton
    {
        private static readonly Lazy<ClientSingleton> lazy =
            new Lazy<ClientSingleton>(() => new ClientSingleton());

        public static ClientSingleton Instance => lazy.Value;
        public ListWrapper<ServerUserModel> Users { get; set; }

        private ClientSingleton()
        {
            Users = new ListWrapper<ServerUserModel>();
            Users.AfterRemove += AfterUserDisconnect;
        }

        private void AfterUserDisconnect(object sender, AfterRemoveEventArgs<ServerUserModel> e)
        {
            foreach (var room in RoomSingleton.Instance.RoomInstances.GetList())
            {
                room.Users.Remove(e.Item);
            }
        }
    }

}
