using System.Collections.Generic;

namespace SharpDj.Common.Handlers.Dictionaries
{
    public interface IDictionaryConverter<in TBase>
    {
        TBag Get<TBag>(IEnumerable<TBase> bagType)
            where TBag : TBase;
    }
}