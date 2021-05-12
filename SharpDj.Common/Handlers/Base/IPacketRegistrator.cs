using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using Network.Interfaces;
using Network.Packets;
using SharpDj.Common.Handlers.Dictionaries.Bags;

namespace SharpDj.Common.Handlers.Base
{
    public interface IAction
    {
        void RegisterPacket(Connection conn);
    }

    public interface IAction<T> where T : RequestPacket
    {
        IHandler Pipeline { get; }
        Task ProcessRequest(T req, Connection conn, IList<IActionBag> actionBags);
    }
}