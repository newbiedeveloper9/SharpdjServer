﻿using System;
using Hik.Communication.Scs.Communication;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Server;

namespace Server.ServerManagment
{
    /// <summary>
    /// Utils class is singleton.
    /// </summary>
    public sealed class Utils
    {
        private static readonly Lazy<Utils> lazy =
            new Lazy<Utils>(() => new Utils());

        public static Utils Instance => lazy.Value;

        private Utils()
        {
        }

        public void SendMessageToAllClients(IScsServer server, string message)
        {
            foreach (var clients in server.Clients.GetAllItems())
            {
                if (clients != null && clients.CommunicationState == CommunicationStates.Connected)
                    clients.SendMessage(new ScsTextMessage(message));
            }
        }

        public string GetIpOfClient(IScsServerClient client)
        {
            var all = client.RemoteEndPoint.ToString();
            var ipStart = all.IndexOf("//");
            var ipWithPort = all.Substring(ipStart + 2);
            var portStart = ipWithPort.IndexOf(":");
            var ip = ipWithPort.Remove(portStart);
            return ip;
        }

        
        
        public int GetPortOfClient(IScsServerClient client)
        {
            var all = client.RemoteEndPoint.ToString();
            var portStart = all.LastIndexOf(":");
            var port = all.Substring(portStart + 1);

            return Convert.ToInt32(port);
        }
    
        /// <param name="client"></param>
        /// <returns> true if given client is already signed in.</returns>
        public bool IsActiveLogin(IScsServerClient client)
        {
             return DataSingleton.Instance.ServerClients[(int)client.ClientId] != null;
        }
    }
}