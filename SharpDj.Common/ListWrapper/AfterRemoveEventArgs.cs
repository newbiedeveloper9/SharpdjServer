using System.ComponentModel;

namespace SharpDj.Common.ListWrapper
{
    public class AfterRemoveEventArgs<Type> : AddingNewEventArgs
    {
        public AfterRemoveEventArgs(Type item)
        {
            Item = item;
        }

        public Type Item { get; private set; }
    }
}
