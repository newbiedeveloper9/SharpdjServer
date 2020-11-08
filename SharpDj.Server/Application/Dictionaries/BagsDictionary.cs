using System.Collections.Generic;
using System.Linq;
using SharpDj.Server.Application.Dictionaries.Bags;

namespace SharpDj.Server.Application.Dictionaries
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
