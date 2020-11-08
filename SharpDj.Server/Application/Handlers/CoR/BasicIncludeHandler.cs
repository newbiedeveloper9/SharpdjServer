using System.Collections.Generic;
using System.Threading.Tasks;
using Network;
using SharpDj.Server.Application.Bags;
using SharpDj.Server.Application.Dictionaries.Bags;
using SharpDj.Server.Application.Models;
using SharpDj.Server.Management.HandlersAction;
using SharpDj.Server.Models;
using SharpDj.Server.Singleton;

namespace SharpDj.Server.Application.Handlers.CoR
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
