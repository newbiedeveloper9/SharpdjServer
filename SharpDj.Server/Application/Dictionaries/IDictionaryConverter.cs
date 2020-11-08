using System.Collections.Generic;

namespace SharpDj.Server.Application.Dictionaries
{
    public interface IDictionaryConverter<TBase>
    {
        TBag Get<TBag>(IEnumerable<TBase> bagType)
            where TBag : TBase;
    }
}