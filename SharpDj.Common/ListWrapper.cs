using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Network.Attributes;

namespace SharpDj.Common
{
    public class ListWrapper<TObj>
    {
        public List<TObj> Wrapper { get; set; }

        public ListWrapper()
        {
            Wrapper = new List<TObj>();
        }

        #region Implementation
        [PacketIgnoreProperty] 
        public int Count => Wrapper.Count();

        public IReadOnlyCollection<TObj> GetList() =>
            Wrapper.AsReadOnly();

        public void Add(TObj obj, bool onlyOneInstance = true)
        {
            if (onlyOneInstance)
                if (Wrapper.Contains(obj)) return;

            OnBeforeUpdate(new UpdateEventArgs(obj, UpdateEventArgs.UpdateState.Insert));
            Wrapper.Add(obj);
            OnAfterUpdate(new UpdateEventArgs(obj, UpdateEventArgs.UpdateState.Insert));
        }

        public void AddRange(List<TObj> obj, bool onlyOneInstance = true) =>
            obj.ForEach((item) => Add(item, onlyOneInstance));

        public bool Remove(TObj obj)
        {
            OnBeforeUpdate(new UpdateEventArgs(obj, UpdateEventArgs.UpdateState.Remove));
            var result = Wrapper.Remove(obj);
            OnAfterUpdate(new UpdateEventArgs(obj, UpdateEventArgs.UpdateState.Remove));

            return result;
        }

        public void RemoveAll(List<TObj> obj) =>
            obj.ForEach((item) => Remove(item));

        #endregion Implementation

        #region Events
        public event EventHandler<UpdateEventArgs> AfterUpdate;
        protected virtual void OnAfterUpdate(UpdateEventArgs e)
        {
            var handler = AfterUpdate;
            handler?.Invoke(this, e);

            if(e.State == UpdateEventArgs.UpdateState.Remove)
                OnAfterRemove(new AfterRemoveEventArgs(e.Item));
            else
                OnAfterAdd(new AfterAddEventArgs(e.Item));
        }


        public event EventHandler<UpdateEventArgs> BeforeUpdate;
        protected virtual void OnBeforeUpdate(UpdateEventArgs e)
        {
            var handler = BeforeUpdate;
            handler?.Invoke(this, e);
        }

        public event EventHandler<AfterAddEventArgs> AfterAdd;
        protected virtual void OnAfterAdd(AfterAddEventArgs e)
        {
            var handler = AfterAdd;
            handler?.Invoke(this, e);
        }

        public event EventHandler<AfterRemoveEventArgs> AfterRemove;
        protected virtual void OnAfterRemove(AfterRemoveEventArgs e)
        {
            var handler = AfterRemove;
            handler?.Invoke(this, e);
        }

        public class UpdateEventArgs : AddingNewEventArgs
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

        public class AfterAddEventArgs : AddingNewEventArgs
        {
            public AfterAddEventArgs(TObj item)
            {
                Item = item;
            }

            public TObj Item { get; private set; }
        }

        public class AfterRemoveEventArgs : AddingNewEventArgs
        {
            public AfterRemoveEventArgs(TObj item)
            {
                Item = item;
            }

            public TObj Item { get; private set; }
        }

        #endregion Events
    }
}
