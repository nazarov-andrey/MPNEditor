using System;
using UnityEngine;

namespace Planner.Input
{
    public class MouseEventArgs : EventArgs
    {
        public readonly Vector2 Position;

        public MouseEventArgs (Vector2 position)
        {
            Position = position;
        }
    }

    public class MouseButtonDownTrigger : InputTrigger<MouseButtonDownTrigger, int, MouseEventArgs>
    {
        public override bool AreConditionsMet (out MouseEventArgs eventArgs)
        {
            eventArgs = new MouseEventArgs (UnityEngine.Input.mousePosition);
            return UnityEngine.Input.GetMouseButtonDown (args);
        }
    }
}