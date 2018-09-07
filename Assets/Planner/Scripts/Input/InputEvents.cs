using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UnityInput = UnityEngine.Input;

namespace Planner.Input
{
    public class KeyEventArgs : EventArgs
    {
        public readonly KeyCode KeyCode;

        public KeyEventArgs (KeyCode keyCode)
        {
            KeyCode = keyCode;
        }
    }

    public class PointerMovedEventArgs : EventArgs
    {
        public readonly Vector2 PrevPosition;
        public readonly Vector2 Position;

        public Vector2 PositionDelta => PrevPosition - Position;

        public PointerMovedEventArgs (Vector2 prevPosition, Vector2 position)
        {
            PrevPosition = prevPosition;
            Position = position;
        }
    }

    public class PointerButtonEventArgs : EventArgs
    {
        public readonly int Button;

        public PointerButtonEventArgs (int button)
        {
            Button = button;
        }
    }

    public class InputEvents : ITickable
    {
        private Dictionary<KeyCode, EventHandler<KeyEventArgs>> keyDown;
        private Dictionary<KeyCode, EventHandler<KeyEventArgs>> key;
        private Dictionary<KeyCode, EventHandler<KeyEventArgs>> keyUp;

        public event EventHandler<PointerMovedEventArgs> PointerMoved;
        public event EventHandler<PointerButtonEventArgs> PointerButtonDown;
        public event EventHandler<PointerButtonEventArgs> PointerButton;
        public event EventHandler<PointerButtonEventArgs> PointerButtonUp;

        private Vector3 prevMousePosition;

        private void OnKeyDown (KeyCode keyCode)
        {
            keyDown[keyCode]?.Invoke (this, new KeyEventArgs (keyCode));
        }

        private void OnKey (KeyCode keyCode)
        {
            key[keyCode]?.Invoke (this, new KeyEventArgs (keyCode));
        }

        private void OnKeyUp (KeyCode keyCode)
        {
            keyUp[keyCode]?.Invoke (this, new KeyEventArgs (keyCode));
        }

        private void OnPointerMoved (PointerMovedEventArgs e)
        {
            PointerMoved?.Invoke (this, e);
        }

        private void OnPointerButtonDown (PointerButtonEventArgs e)
        {
            PointerButtonDown?.Invoke (this, e);
        }

        private void OnPointerButton (PointerButtonEventArgs e)
        {
            PointerButton?.Invoke (this, e);
        }

        private void OnPointerButtonUp (PointerButtonEventArgs e)
        {
            PointerButtonUp?.Invoke (this, e);
        }

        private void AddKeyListener (
            Dictionary<KeyCode, EventHandler<KeyEventArgs>> @event,
            KeyCode keyCode,
            EventHandler<KeyEventArgs> listener)
        {
            if (@event.ContainsKey (keyCode))
                @event[keyCode] += listener;
            else
                @event[keyCode] = listener;
        }

        private void RemoveKeyListener (
            Dictionary<KeyCode, EventHandler<KeyEventArgs>> @event,
            KeyCode keyCode,
            EventHandler<KeyEventArgs> listener)
        {
            if (@event.ContainsKey (keyCode))
                @event[keyCode] -= listener;
        }

        private void CheckKeyEvent (
            Dictionary<KeyCode, EventHandler<KeyEventArgs>> listeners,
            Predicate<KeyCode> predicate,
            Action<KeyCode> action)
        {
            foreach (var keyCode in listeners.Keys.ToArray ()) {
                if (predicate (keyCode))
                    action (keyCode);
            }
        }

        public void AddKeyDownListener (KeyCode keyCode, EventHandler<KeyEventArgs> listener)
        {
            AddKeyListener (keyDown, keyCode, listener);
        }

        public void AddKeyListener (KeyCode keyCode, EventHandler<KeyEventArgs> listener)
        {
            AddKeyListener (keyDown, keyCode, listener);
        }

        public void AddKeyUpListener (KeyCode keyCode, EventHandler<KeyEventArgs> listener)
        {
            AddKeyListener (keyDown, keyCode, listener);
        }

        public void RemoveKeyDownListener (KeyCode keyCode, EventHandler<KeyEventArgs> listener)
        {
            RemoveKeyListener (keyDown, keyCode, listener);
        }

        public void RemoveKeyListener (KeyCode keyCode, EventHandler<KeyEventArgs> listener)
        {
            RemoveKeyListener (keyDown, keyCode, listener);
        }

        public void RemoveKeyUpListener (KeyCode keyCode, EventHandler<KeyEventArgs> listener)
        {
            RemoveKeyListener (keyDown, keyCode, listener);
        }

        public void Tick ()
        {
            for (int i = 0; i < 3; i++) {
                if (UnityInput.GetMouseButtonDown (i))
                    OnPointerButtonDown (new PointerButtonEventArgs (i));

                if (UnityInput.GetMouseButton (i))
                    OnPointerButton (new PointerButtonEventArgs (i));

                if (UnityInput.GetMouseButtonUp (i))
                    OnPointerButtonUp (new PointerButtonEventArgs (i));
            }

            CheckKeyEvent (keyDown, UnityInput.GetKeyDown, OnKeyDown);
            CheckKeyEvent (key, UnityInput.GetKey, OnKey);
            CheckKeyEvent (keyUp, UnityInput.GetKeyUp, OnKeyUp);

            var mousePosition = UnityInput.mousePosition;
            if (prevMousePosition != mousePosition)
                OnPointerMoved (new PointerMovedEventArgs (prevMousePosition, mousePosition));

            prevMousePosition = UnityInput.mousePosition;
        }
    }
}