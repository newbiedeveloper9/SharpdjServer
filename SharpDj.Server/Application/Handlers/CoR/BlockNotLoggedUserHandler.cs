using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDj.Server.Application.Dictionaries;
using SharpDj.Server.Application.Dictionaries.Bags;

namespace SharpDj.Server.Application.Handlers.CoR
{
    public class BlockNotLoggedUserHandler : AbstractHandler
    {

        public BlockNotLoggedUserHandler(IDictionaryConverter<IActionBag> bagConverter)
            : base(bagConverter)
        {
        }

        public override async Task<object> Handle(object request, List<IActionBag> actionBags)
        {
            var user = BagConverter.Get<ActiveUserBag>(actionBags);

            if (user == null)
            {
                return "User is not logged in";
            }

            return await base.Handle(request, actionBags);
        }
    }
}
