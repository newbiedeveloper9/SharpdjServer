using System;
using System.ComponentModel;
using Server.Management;

namespace Server
{
    public class SpecialAfterInsertList<TRoom> : BindingList<TRoom>

    {
    protected override void InsertItem(int index, TRoom item)
    {
        base.InsertItem(index, item);
        OnAfterInsert(index, item);
    }

    public event EventHandler<SpecialAfterInsertListEventArgs> AfterInsert;

    protected virtual void OnAfterInsert(int index, TRoom item)
    {
        var handler = AfterInsert;
        handler?.Invoke(this, new SpecialAfterInsertListEventArgs(index, item));
    }

    public class SpecialAfterInsertListEventArgs : AddingNewEventArgs
    {
        public SpecialAfterInsertListEventArgs(int index, object item)
        {
            Index = index;
            Item = item;
        }

        public int Index { get; private set; }
        public object Item { get; private set; }
    }
    }
}
