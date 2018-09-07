using System;
using UnityEngine;

namespace Planner.Input
{
    public class KeyUpTrigger : InputTrigger<KeyUpTrigger, KeyCode, EventArgs>
    {
        public override bool AreConditionsMet (out EventArgs eventArgs)
        {
            eventArgs = EventArgs.Empty;
            return UnityEngine.Input.GetKeyUp (args);
        }
    }
}