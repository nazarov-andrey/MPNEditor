using System;
using Zenject;

namespace Planner.Input
{
    public abstract class InputTrigger<TSelf, TArgs, TEventArgs> : ITickable
        where TSelf : InputTrigger<TSelf, TArgs, TEventArgs>, new ()
        where TEventArgs : EventArgs
    {
        public class Factory : IInputTriggerFactory
        {
            [Inject]
            private TickableManager tickableManager;

            public TSelf Create (TArgs args = default (TArgs))
            {
                var instance = new TSelf
                {
                    tickableManager = tickableManager,
                    args = args
                };

                return instance;
            }
        }

        private TickableManager tickableManager;
        protected TArgs args;

        public event EventHandler<TEventArgs> Fired;

        protected virtual void OnFired (TEventArgs e)
        {
            Fired?.Invoke (this, e);
        }

        public abstract bool AreConditionsMet (out TEventArgs eventArgs);

        public virtual void Arm ()
        {
            tickableManager.Add (this);
        }

        public virtual void Disarm ()
        {
            tickableManager.Remove (this);
        }

        public void Tick ()
        {
            TEventArgs eventArgs;
            if (AreConditionsMet (out eventArgs))
                OnFired (eventArgs);
        }
    }

    public abstract class InputTrigger<TSelf, TEventArgs> : InputTrigger<TSelf, object, TEventArgs>
        where TSelf : InputTrigger<TSelf, TEventArgs>, new ()
        where TEventArgs : EventArgs
    {
    }
}