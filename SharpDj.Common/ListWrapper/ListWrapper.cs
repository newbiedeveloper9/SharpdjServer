using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpDj.Common.ListWrapper
{
    public class ListWrapper<TObj>
    {
        public List<TObj> Wrapper { get; private set; }

        public ListWrapper()
        {
            Wrapper = new List<TObj>();
        }

        #region Implementation

        public IReadOnlyCollection<TObj> ToReadonlyList() =>
            Wrapper.AsReadOnly();

        public void Add(TObj obj, bool onlyOneInstance = true)
        {
            if (onlyOneInstance)
                if (Wrapper.Contains(obj)) return;

            OnBeforeUpdate(new UpdateEventArgs<TObj>(obj, UpdateEventArgs<TObj>.UpdateState.Insert));
            Wrapper.Add(obj);
            OnAfterUpdate(new UpdateEventArgs<TObj>(obj, UpdateEventArgs<TObj>.UpdateState.Insert));
        }

        public void AddRange(List<TObj> obj, bool onlyOneInstance = true) =>
            obj.ForEach((item) => Add(item, onlyOneInstance));

        public bool Remove(TObj obj)
        {
            OnBeforeUpdate(new UpdateEventArgs<TObj>(obj, UpdateEventArgs<TObj>.UpdateState.Remove));
            var result = Wrapper.Remove(obj);
            OnAfterUpdate(new UpdateEventArgs<TObj>(obj, UpdateEventArgs<TObj>.UpdateState.Remove));

            return result;
        }

        public void RemoveAll(List<TObj> obj) =>
            obj.ForEach((item) => Remove(item));

        #endregion Implementation

        #region LINQ

        public TObj FirstOrDefault()
        {
            return Wrapper.FirstOrDefault();
        }

        public TObj FirstOrDefault(Func<TObj, bool> predicate)
        {
            return Wrapper.FirstOrDefault(predicate);
        }

        public int Count()
        {
            return Wrapper.Count();
        }

        public int Count(Func<TObj, bool> predicate)
        {
            return Wrapper.Count(predicate);
        }

        public bool Any()
        {
            return Wrapper.Any();
        }

        public bool Any(Func<TObj, bool> predicate)
        {
            return Wrapper.Any(predicate);
        }

        #endregion LINQ

        #region Events
        public event EventHandler<UpdateEventArgs<TObj>> AfterUpdate;
        protected virtual void OnAfterUpdate(UpdateEventArgs<TObj> e)
        {
            var handler = AfterUpdate;
            handler?.Invoke(this, e);

            if (e.State == UpdateEventArgs<TObj>.UpdateState.Remove)
                OnAfterRemove(new AfterRemoveEventArgs<TObj>(e.Item));
            else
                OnAfterAdd(new AfterAddEventArgs<TObj>(e.Item));
        }


        public event EventHandler<UpdateEventArgs<TObj>> BeforeUpdate;
        protected virtual void OnBeforeUpdate(UpdateEventArgs<TObj> e)
        {
            var handler = BeforeUpdate;
            handler?.Invoke(this, e);
        }

        public event EventHandler<AfterAddEventArgs<TObj>> AfterAdd;
        protected virtual void OnAfterAdd(AfterAddEventArgs<TObj> e)
        {
            var handler = AfterAdd;
            handler?.Invoke(this, e);
        }

        public event EventHandler<AfterRemoveEventArgs<TObj>> AfterRemove;
        protected virtual void OnAfterRemove(AfterRemoveEventArgs<TObj> e)
        {
            var handler = AfterRemove;
            handler?.Invoke(this, e);
        }

        #endregion Events
    }
}
