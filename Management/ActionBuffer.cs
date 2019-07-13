﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Network;
using Network.Packets;
using SCPackets.Buffers;

namespace Server.Management
{
    public class ActionBuffer<TReq> where TReq : RequestPacket

    {
        public List<Connection> Connections { get; set; }
        public TReq RequestPacket { get; set; }

        public ActionBuffer(int timer, TReq packet)
        {
            Connections = new List<Connection>();
            RequestPacket = packet;
        }

        public void SendRequestToAll()
        {
            OnBeforeSendBuffer(EventArgs.Empty);

            foreach (var connection in Connections)
                connection.Send(RequestPacket);
        }

        public event EventHandler BeforeSendBuffer;

        protected virtual void OnBeforeSendBuffer(EventArgs e)
        {
            var handler = BeforeSendBuffer;
            handler?.Invoke(this, e);
        }
    }
}
