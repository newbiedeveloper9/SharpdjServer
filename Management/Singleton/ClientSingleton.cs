﻿using System;
using System.Collections.Generic;
using System.Linq;
using SCPackets;

namespace Server.Management.Singleton
{
    public sealed class ClientSingleton
    {
        private static readonly Lazy<ClientSingleton> lazy =
            new Lazy<ClientSingleton>(() => new ClientSingleton());

        public static ClientSingleton Instance => lazy.Value;

        private ClientSingleton()
        {
            Users = new ListWrapper<ServerUserModel>();
            Users.AfterRemove += AfterUserDisconnect;
        }

        private void AfterUserDisconnect(object sender, ListWrapper<ServerUserModel>.AfterRemoveEventArgs e)
        {
            foreach (var room in RoomSingleton.Instance.RoomInstances.GetList())
            {
                room.Users.Remove(e.Item);
            }
        }

        public ListWrapper<ServerUserModel> Users { get; set; }
    }
}
