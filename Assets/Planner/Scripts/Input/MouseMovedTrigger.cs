using System;
using UnityEngine;

namespace Planner.Input
{
    public class MouseMovedEventArgs : EventArgs
    {
        public readonly Vector2 PrevPosition;
        public readonly Vector2 Position;

        public MouseMovedEventArgs (Vector2 prevPosition, Vector2 position)
        {
            PrevPosition = prevPosition;
            Position = position;
        }
    }

    public class MouseMovedTrigger : InputTrigger<MouseMovedTrigger, MouseMovedEventArgs>
    {
        private Vector3 prevPosition;
        private Vector3 position;

        public Vector3 PrevPosition => prevPosition;

        public Vector3 Position => position;

        private Vector3 GetMousePosition ()
        {
            return UnityEngine.Input.mousePosition;
        }

        public override bool AreConditionsMet (out MouseMovedEventArgs eventArgs)
        {
            eventArgs = null;
            position = GetMousePosition ();
            if (prevPosition == position)
                return false;

            eventArgs = new MouseMovedEventArgs (prevPosition, position);
            return true;
        }

        protected override void OnFired (MouseMovedEventArgs mouseMovedEventArgs)
        {
            base.OnFired (mouseMovedEventArgs);
            prevPosition = position;
        }

        public override void Arm ()
        {
            base.Arm ();
            prevPosition = GetMousePosition ();
        }
    }
}