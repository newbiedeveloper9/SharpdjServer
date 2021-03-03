using System.Collections.Generic;
using System.Linq;
using SharpDj.Common.Handlers.Dictionaries.Bags;

namespace SharpDj.Common.Handlers.Dictionaries
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
