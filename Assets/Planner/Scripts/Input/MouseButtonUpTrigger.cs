using System;

namespace Planner.Input
{
    public class MouseButtonUpTrigger : InputTrigger<MouseButtonUpTrigger, int, MouseEventArgs>
    {
        public override bool AreConditionsMet (out MouseEventArgs eventArgs)
        {
            eventArgs = new MouseEventArgs (UnityEngine.Input.mousePosition);
            return UnityEngine.Input.GetMouseButtonUp (args);
        }
    }
}