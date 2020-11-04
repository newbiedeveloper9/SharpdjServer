using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDj.Server.Management.HandlersAction;

namespace SharpDj.Server.Application.Handlers.CoR
{
    public interface IHandler
    {
        IHandler SetNext(IHandler handler);
        Task<object> Handle(object request, List<IActionBag> actionBags);
    }
}