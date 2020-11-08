using System.Collections.Generic;
using System.Threading.Tasks;
using SharpDj.Server.Application.Dictionaries;
using SharpDj.Server.Application.Dictionaries.Bags;

namespace SharpDj.Server.Application.Handlers.CoR
{
    public class BlockLoggedUserHandler : AbstractHandler
    {

        public BlockLoggedUserHandler(IDictionaryConverter<IActionBag> bagConverter)
            : base(bagConverter)
        {
        }

        public override async Task<object> Handle(object request, List<IActionBag> actionBags)
        {
            var user = BagConverter.Get<ActiveUserBag>(actionBags);
            if (user != null)
            {
                return $"Given connection is already logged in to account {user.ActiveUser.UserEntity.Username}";
            }

            return await base.Handle(request, actionBags);
        }
    }
}