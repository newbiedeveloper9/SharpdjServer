using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using SharpDj.Common.Handlers;
using SharpDj.Common.Handlers.Dictionaries;
using SharpDj.Common.Handlers.Dictionaries.Bags;
using SharpDj.Server.Application.Models;
using SharpDj.Server.Singleton;
using ActiveUserBag = SharpDj.Server.Application.Commands.Bags.ActiveUserBag;
using ConnectionBag = SharpDj.Server.Application.Commands.Bags.ConnectionBag;

namespace SharpDj.Server.Application.Commands.Extensions
{
    public class BasicIncludeHandler : AbstractHandler
    {

        public BasicIncludeHandler(IDictionaryConverter<IActionBag> bagConverter)
        : base(bagConverter)
        {
        }

        public override async Task<object> Handle(object request, List<IActionBag> actionBags)
        {
            var connectionBag = BagConverter.Get<ConnectionBag>(actionBags);
            var active = GetActiveUser(connectionBag.Connection);

            if (active?.UserEntity != null)
            {
                actionBags.Add(new ActiveUserBag(active));
            }

            return await base.Handle(request, actionBags);
        }

        private ServerUserModel GetActiveUser(Connection conn)
        {
            return ClientSingleton.Instance.Users
                .FirstOrDefault(x => x.Connections.Contains(conn));
        }
    }
}
