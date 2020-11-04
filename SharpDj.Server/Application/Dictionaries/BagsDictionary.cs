using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDj.Server.Management.HandlersAction;

namespace SharpDj.Server.Application.Bags
{
    public class BagsDictionary : IDictionaryConverter<IActionBag>
    {
        public TBag Get<TBag>(IEnumerable<IActionBag> bag) 
            where TBag : IActionBag
        {
            var result = bag.FirstOrDefault(x => x.GetType() == typeof(TBag));
            return (TBag)result;
        }
    }
}
