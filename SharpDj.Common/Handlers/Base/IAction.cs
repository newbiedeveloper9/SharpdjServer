using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using Network.Packets;
using SharpDj.Common.Handlers.Dictionaries.Bags;

namespace SharpDj.Common.Handlers.Base
{
    public interface IAction<in T> where T : RequestPacket
    {
        IHandler BuildPipeline();
        Task ProcessRequest(T req, Connection conn, IList<IActionBag> actionBags);
    }
}