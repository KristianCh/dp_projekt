using System;
using System.Collections.Generic;
using System.Linq;

namespace Entities.Events
{
    
    /// <summary>
    /// Signal class without parameters.
    /// </summary>
    public class Signal : ASignal
    {
        public event Action Listener = null;
        public event Action OnceListener = null;

        public void AddListener(Action callback)
        {
            Listener = this.AddUnique(Listener, callback);
        }

        public void AddOnce(Action callback) { OnceListener = this.AddUnique(OnceListener, callback); }

        public void RemoveListener(Action callback)
        {
            if (Listener != null)
                Listener -= callback;
        }

        public void RemoveOnceListener(Action callback)
        {
            if (OnceListener != null)
                OnceListener -= callback;
        }

        public override List<Type> GetTypes() { return new List<Type>(); }

        public virtual void Dispatch()
        {
            Listener?.Invoke();
            OnceListener?.Invoke();
            OnceListener = null;
            base.Dispatch(null);
        }

        protected Action AddUnique(Action listeners, Action callback)
        {
            if (listeners == null || !listeners.GetInvocationList().Contains(callback))
            {
                listeners += callback;
            }

            return listeners;
        }

        public override void RemoveAllListeners()
        {
            Listener = null;
            OnceListener = null;
            base.RemoveAllListeners();
        }

        public Delegate listener { get { return Listener ?? (Listener = delegate { }); } set { Listener = (Action) value; } }
    }

    /// <summary>
    /// Signal class with one generic parameter.
    /// </summary>
    public class Signal<T> : ASignal
    {
        public event Action<T> Listener = null;
        public event Action<T> OnceListener = null;

        public void AddListener(Action<T> callback) { Listener = this.AddUnique(Listener, callback); }

        public void AddOnce(Action<T> callback) { OnceListener = this.AddUnique(OnceListener, callback); }

        public void RemoveListener(Action<T> callback)
        {
            if (Listener != null)
                Listener -= callback;
        }

        public void RemoveOnceListener(Action<T> callback)
        {
            if (OnceListener != null)
                OnceListener -= callback;
        }

        public override List<Type> GetTypes()
        {
            List<Type> retv = new List<Type>();
            retv.Add(typeof(T));
            return retv;
        }

        public void Dispatch(T type1)
        {
            Listener?.Invoke(type1);
            OnceListener?.Invoke(type1);
            OnceListener = null;
            object[] outv = {type1};
            base.Dispatch(outv);
        }

        private Action<T> AddUnique(Action<T> listeners, Action<T> callback)
        {
            if (listeners == null || !listeners.GetInvocationList().Contains(callback))
            {
                listeners += callback;
            }

            return listeners;
        }

        public override void RemoveAllListeners()
        {
            Listener = null;
            OnceListener = null;
            base.RemoveAllListeners();
        }

        public Delegate listener { get { return Listener ?? (Listener = delegate { }); } set { Listener = (Action<T>) value; } }
    }
}