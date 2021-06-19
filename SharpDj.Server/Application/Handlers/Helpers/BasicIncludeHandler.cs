using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using SharpDj.Common.Handlers;
using SharpDj.Common.Handlers.Dictionaries;
using SharpDj.Common.Handlers.Dictionaries.Bags;
using SharpDj.Server.Application.Bags;
using SharpDj.Server.Application.Commands.Bags;
using SharpDj.Server.Application.Models;
using SharpDj.Server.Singleton;

namespace SharpDj.Server.Application.Handlers.Helpers
{
    public class BasicIncludeHandler : AbstractHandler
    {
        public BasicIncludeHandler(IDictionaryConverter<IActionBag> bagConverter)
        : base(bagConverter)
        {
        }

        public override async Task<object> Handle(object request, IList<IActionBag> actionBags)
        {
            var connectionBag = BagConverter.Get<ConnectionBag>(actionBags);
            var active = GetActiveUser(connectionBag.Connection);

            if (active?.UserEntity != null)
            {
                actionBags.Add(new ActiveUserBag(active));
            }

            return await base.Handle(request, actionBags)
                .ConfigureAwait(false);
        }

        private ServerUserModel GetActiveUser(Connection conn)
        {
            return ClientSingleton.Instance.Users
                .FirstOrDefault(x => x.Connections.Contains(conn));
        }
    }
}
