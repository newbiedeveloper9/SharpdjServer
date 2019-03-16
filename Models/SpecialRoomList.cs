using System;
using System.ComponentModel;

namespace Server
{
    public class SpecialRoomList<TRoom> : BindingList<TRoom> where TRoom : RoomInstance
    {
        protected override void InsertItem(int index, TRoom item)
        {
            base.InsertItem(index, item);
            OnAfterInsert(index, item);
        }

        public event EventHandler<SpecialRoomArgs> AfterInsert;

        protected virtual void OnAfterInsert(int index, TRoom item)
        {
            var handler = AfterInsert;
            handler?.Invoke(this, new SpecialRoomArgs(index, item));
        }

        public class SpecialRoomArgs : AddingNewEventArgs
        {
            public SpecialRoomArgs(int index, RoomInstance item)
            {
                Index = index;
                Item = item;
            }

            public int Index { get; private set; }
            public RoomInstance Item { get; private set; }
        }
    }
}
