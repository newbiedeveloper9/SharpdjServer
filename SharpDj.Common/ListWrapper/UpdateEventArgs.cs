using System.ComponentModel;

namespace SharpDj.Common.ListWrapper
{
    public class UpdateEventArgs<TObj> : AddingNewEventArgs
    {
        public UpdateEventArgs(TObj item, UpdateState state)
        {
            Item = item;
            State = state;
        }

        public TObj Item { get; private set; }
        public UpdateState State { get; private set; }

        public enum UpdateState
        {
            Remove,
            Insert
        }
    }
}
