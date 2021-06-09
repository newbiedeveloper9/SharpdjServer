using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDj.Common.Handlers;
using SharpDj.Common.Handlers.Dictionaries;
using SharpDj.Common.Handlers.Dictionaries.Bags;
using SharpDj.Server.Application.Commands.Bags;

namespace SharpDj.Server.Application.Commands.Extensions
{
    public class BlockNotLoggedUserHandler : AbstractHandler
    {
        public BlockNotLoggedUserHandler(IDictionaryConverter<IActionBag> bagConverter)
            : base(bagConverter)
        {
        }

        public override async Task<object> Handle(object request, IList<IActionBag> actionBags)
        {
            var user = BagConverter.Get<ActiveUserBag>(actionBags);

            if (user == null)
            {
                return "User is not logged in";
            }

            return await base.Handle(request, actionBags)
                .ConfigureAwait(false);
        }
    }
}
