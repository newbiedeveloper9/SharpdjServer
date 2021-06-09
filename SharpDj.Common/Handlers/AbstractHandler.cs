using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharpDj.Common.Handlers.Base;
using SharpDj.Common.Handlers.Dictionaries;
using SharpDj.Common.Handlers.Dictionaries.Bags;
using SharpDj.Server.Application.Commands.Bags;

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

        public virtual async Task<object> Handle(object request, IList<IActionBag> actionBags)
        {
            if (_successor == null)
            {
                return null;
            }

            await HandleWhenRequest(request, actionBags)
                .ConfigureAwait(false);

            return await _successor.Handle(request, actionBags)
                .ConfigureAwait(false);
        }

        private async Task HandleWhenRequest(object request, IList<IActionBag> actionBags)
        {
            var requestHandlerType = _successor
                .GetType().GetInterfaces()
                .FirstOrDefault(x => x.IsGenericType &&
                                     x.GetGenericTypeDefinition() == typeof(IAction<>));
            if (requestHandlerType != null)
            {
                var connection = BagConverter.Get<ConnectionBag>(actionBags).Connection;

                var castRequest = CastGenericRequest(request, requestHandlerType);

                dynamic requestAction = _successor;
                await requestAction.ProcessRequest(castRequest, connection, actionBags);
            }
        }

        private dynamic CastGenericRequest(object request, Type requestType)
        {
            var parameterType = requestType.GenericTypeArguments.FirstOrDefault();

            if (parameterType is null)
            {
                throw new Exception("CastGenericRequest: Problem with request object.");
            }

            return Convert.ChangeType(request, parameterType);
        }
    }
}
