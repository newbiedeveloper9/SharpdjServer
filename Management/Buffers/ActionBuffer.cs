using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Network;
using Network.Packets;
using NLog;
using SCPackets.Buffers;

namespace Server.Management
{
    public class ActionBuffer<TReq> where TReq : RequestPacket

    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public List<Connection> Connections { get; set; }
        public TReq RequestPacket { get; set; }

        /// <summary>
        /// <para>After <see cref="OnBeforeSendBuffer"/> is done, this property change to <see langword="TRUE"/>. </para>
        /// <para>It sends <see cref="RequestPacket"/> only if this variable is <see langword="TRUE"/>. </para>
        /// </summary>  
        public bool CanSend { get; set; } = true;

        public ActionBuffer(TReq packet)
        {
            Connections = new List<Connection>();
            RequestPacket = packet;
        }

        /// <summary>
        /// <para>Checks if Request is ready, then sends to every connection in <see cref="Connections"/>.</para>
        /// <para>It invokes <see cref="OnBeforeSendBuffer(EventArgs)"/> at the beginning.</para>
        ///     <returns> <para>
        ///         <see langword="TRUE"/> if success, otherwise returns <see langword="FALSE"/>
        ///     </para></returns>
        /// </summary>

        public bool SendRequestToEveryone()
        {
            OnBeforeSendBuffer(EventArgs.Empty);
            if (!CanSend) return false;

            foreach (var connection in Connections)
                connection.Send(RequestPacket);
            Logger.Info("Buffer data has been sent");
            return true;
        }

        public event EventHandler BeforeSendBuffer;

        /// <summary>
        /// <para>Set custom logic to do, before packet is sent</para>
        /// </summary>
        protected virtual void OnBeforeSendBuffer(EventArgs e)
        {
            Logger.Trace("Before buffer data has been sent...");
            var handler = BeforeSendBuffer;
            handler?.Invoke(this, e);
        }
    }
}
