using System;
using UnityEngine;

namespace Planner.Input
{
    public class KeyTrigger : InputTrigger<KeyTrigger, KeyCode, EventArgs>
    {
        public override bool AreConditionsMet (out EventArgs eventArgs)
        {
            eventArgs = EventArgs.Empty;
            return UnityEngine.Input.GetKey (args);
        }
    }
}