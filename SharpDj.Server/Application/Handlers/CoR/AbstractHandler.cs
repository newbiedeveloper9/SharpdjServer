using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDj.Server.Application.Bags;
using SharpDj.Server.Management.HandlersAction;

namespace SharpDj.Server.Application.Handlers.CoR
{
    public abstract class AbstractHandler : IHandler
    {
        protected readonly IDictionaryConverter<IActionBag> BagConverter;
        private IHandler _successor;

        protected AbstractHandler(IDictionaryConverter<IActionBag> bagConverter)
        {
            BagConverter = bagConverter;
        }

        public IHandler SetNext(IHandler handler)
        {
            _successor = handler;

            return handler;
        }

        public virtual async Task<object> Handle(object request, List<IActionBag> actionBags)
        {
            if (_successor != null)
            {
                return await _successor.Handle(request, actionBags);
            }

            return null;
        }
    }
}
