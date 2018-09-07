using System;
using UnityEngine;

namespace Planner.Input
{
    public class KeyDownTrigger : InputTrigger<KeyDownTrigger, KeyCode, EventArgs>
    {
        public override bool AreConditionsMet (out EventArgs eventArgs)
        {
            eventArgs = EventArgs.Empty;
            return UnityEngine.Input.GetKeyDown (args);
        }
    }
}