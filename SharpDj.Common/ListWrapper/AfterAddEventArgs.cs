using System.ComponentModel;

namespace SharpDj.Common.ListWrapper
{
    public class AfterAddEventArgs<Type> : AddingNewEventArgs
        {
            public AfterAddEventArgs(Type item)
            {
                Item = item;
            }

            public Type Item { get; private set; }
        }
}
