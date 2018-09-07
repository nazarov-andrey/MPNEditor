using System;

namespace Planner.Input
{
    public class MouseButtonTrigger : InputTrigger<MouseButtonTrigger, int, MouseEventArgs>
    {
        public override bool AreConditionsMet (out MouseEventArgs eventArgs)
        {
            eventArgs = new MouseEventArgs (UnityEngine.Input.mousePosition);
            return UnityEngine.Input.GetMouseButton (args);
        }
    }
}