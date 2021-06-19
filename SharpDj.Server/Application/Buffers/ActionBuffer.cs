using System;
using System.Collections.Generic;
using Network;
using Network.Packets;
using Serilog;

namespace SharpDj.Server.Application.Buffers
{
    public class ActionBuffer<TReq> where TReq : RequestPacket

    {
        public List<Connection> Connections { get; set; }
        public TReq RequestPacket { get; set; }

        /// <summary>
        /// After <see cref="OnBeforeSendBuffer"/> is done, this property change to <see langword="TRUE"/>. 
        /// It sends <see cref="RequestPacket"/> only if this variable is <see langword="TRUE"/>.
        /// </summary>  
        public bool CanSend { get; set; } = true;

        public ActionBuffer(TReq packet)
        {
            Connections = new List<Connection>();
            RequestPacket = packet;
        }

        ///<summary>
        ///Checks if Request is ready, then sends to every connection in <see cref="Connections"/>.
        ///It invokes <see cref="OnBeforeSendBuffer(EventArgs)"/> at the beginning.
        ///</summary>
        public bool SendRequestToEveryone()
        {
            OnBeforeSendBuffer(EventArgs.Empty);
            if (!CanSend)
            {
                return false;
            }

            foreach (var connection in Connections)
            {
                connection.Send(RequestPacket);
            }

            Log.Information("Buffer data has been sent");

            return true;
        }

        public event EventHandler BeforeSendBuffer;

        /// <summary>
        /// Set custom logic to do, before packet is sent
        /// </summary>
        protected virtual void OnBeforeSendBuffer(EventArgs e)
        {
            var handler = BeforeSendBuffer;
            handler?.Invoke(this, e);
        }
    }
}
