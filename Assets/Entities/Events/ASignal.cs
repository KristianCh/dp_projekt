using System;
using System.Collections.Generic;
using System.Linq;

namespace Entities.Events
{
    /// <summary>
    /// Class allowing easy subscribing functions to events and dispatching said events.
    /// </summary>
    public class ASignal
    {
        public event Action<ASignal, object[]> BaseListener;

        public event Action<ASignal, object[]> OnceBaseListener;

        public void Dispatch(object[] args)
        {
            if (BaseListener != null)
                BaseListener(this, args);
            if (OnceBaseListener != null)
                OnceBaseListener(this, args);
            OnceBaseListener = null;
        }

        public virtual List<Type> GetTypes()
        {
            return new List<Type>();
        }

        public void AddListener(Action<ASignal, object[]> callback)
        {
            BaseListener = AddUnique(BaseListener, callback);
        }

        public void AddOnce(Action<ASignal, object[]> callback)
        {
            OnceBaseListener = AddUnique(OnceBaseListener, callback);
        }

        private Action<T, U> AddUnique<T, U>(Action<T, U> listeners, Action<T, U> callback)
        {
            if (listeners == null || !listeners.GetInvocationList().Contains(callback))
            {
                listeners += callback;
            }

            return listeners;
        }
        
        public void RemoveListener(Action<ASignal, object[]> callback)
        {
            if (BaseListener != null)
                BaseListener -= callback;
        }
        
        public virtual void RemoveAllListeners()
        {
            BaseListener = null;
            OnceBaseListener = null;
        }
    }
}