using UnityEngine;

namespace Planner
{
    public class CursorLine : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer lineRenderer;

        [SerializeField]
        private Material linePossible;

        [SerializeField]
        private Material lineImpossible;

        public void SetPossible (bool possible)
        {
            lineRenderer.material = possible ? linePossible : lineImpossible;
        }

        public void Refresh (Component start, Component end)
        {
            lineRenderer.SetPosition (0, start.transform.position);
            lineRenderer.SetPosition (1, end.transform.position);
        }
    }
}