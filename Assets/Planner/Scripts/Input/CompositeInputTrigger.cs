using System;

namespace Planner.Input
{
    public class CompositeInputTrigger : InputTrigger<CompositeInputTrigger, IInputTrigger[], EventArgs>
    {
        public override bool AreConditionsMet (out EventArgs eventArgs)
        {
            eventArgs = EventArgs.Empty;
            return Array.TrueForAll (args, x => x.AreConditionsMet ());
        }
    }
}