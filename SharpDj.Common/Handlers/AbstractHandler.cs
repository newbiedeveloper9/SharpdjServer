using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDj.Common.Handlers.Dictionaries;
using SharpDj.Common.Handlers.Dictionaries.Bags;

namespace SharpDj.Common.Handlers
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
