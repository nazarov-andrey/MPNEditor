using Boo.Lang;
using ModestTree;
using UnityEngine;

namespace MPNEditor.Utils
{
    public static class ComponentExtension
    {
        public static void BringFullscreenRectTransformToScreen (this Component component)
        {
            RectTransform rectTransform = component.transform as RectTransform;
            if (rectTransform == null)
                return;

            rectTransform.anchoredPosition = Vector2.zero;
        }

        public static string GetPath (this Component component)
        {
            Transform transform = component.transform;
            var pathChunks = new List<string> ();

            while (transform != null) {
                pathChunks.Insert (0, transform.gameObject.name);
                transform = transform.parent;
            }

            string path = string.Join ("/", pathChunks.ToArray ());
            pathChunks.Clear ();

            return path;
        }

        public static bool IsActiveSelf (this Component component)
        {
            return component
                .gameObject
                .activeSelf;
        }

        public static void SetActive (this Component component, bool active)
        {
            component
                .gameObject
                .SetActive (active);
        }

        public static void Activate (this Component component)
        {
            component
                .gameObject
                .SetActive (true);
        }

        public static void Deactivate (this Component component)
        {
            component
                .gameObject
                .SetActive (false);
        }

        public static int GetSiblingIndex (this Component component)
        {
            return component
                .transform
                .GetSiblingIndex ();
        }

        public static RectTransform GetRectTransform (this Component component)
        {
            return component.transform as RectTransform;
        }

        private static bool IsTypeDerivedFromTransform<T> ()
        {
            return typeof (T).DerivesFromOrEqual (typeof (Transform));
        }

        private static bool InternalGetChild<T> (Component component, out T result, int index = -1) where T : Component
        {
            T[] childs = component.GetComponentsInChildren<T> (true);
            if (index == -1)
                index = childs.Length - 1;

            result = default (T);
            bool success = index >= 0 && index < childs.Length;
            if (success)
                result = childs[index];
            return success;
        }

        private static bool GetTransformChild<T> (Component component, int index, out T result) where T : Component
        {
            result = default (T);
            int childCount = component.transform.childCount;
            bool success = childCount > 0 && index >= 0 && index < childCount;
            if (success)
                result = component.transform.GetChild (index) as T;

            return success;
        }

        public static bool GetFirstChild<T> (this Component component, out T result) where T : Component
        {
            result = default (T);
            if (IsTypeDerivedFromTransform<T> ())
                return GetTransformChild (component, 0, out result);

            return InternalGetChild (component, out result, 0);
        }

        public static bool GetChild<T> (this Component component, int index, out T result) where T : Component
        {
            result = default (T);
            if (IsTypeDerivedFromTransform<T> ())
                return GetTransformChild (component, index, out result);

            return InternalGetChild (component, out result, index);
        }

        public static bool GetLastChild<T> (this Component component, out T result) where T : Component
        {
            result = default (T);
            if (IsTypeDerivedFromTransform<T> ())
                return GetTransformChild (component, component.transform.childCount - 1, out result);

            return InternalGetChild (component, out result);
        }

        public static int GetChildCount<T> (this Component component) where T : Component
        {
            if (IsTypeDerivedFromTransform<T> ())
                return component.transform.childCount;

            return component.GetComponentsInChildren<T> ().Length;
        }
    }
}