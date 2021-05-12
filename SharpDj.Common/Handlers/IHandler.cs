using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDj.Common.Handlers.Dictionaries.Bags;

namespace SharpDj.Common.Handlers
{
    public interface IHandler
    {
        IHandler SetNext(IHandler handler);
        Task<object> Handle(object request, IList<IActionBag> actionBags);
    }
}