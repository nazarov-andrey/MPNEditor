using System.Collections.Generic;
using Planner.Model;
using UnityEngine;

namespace Planner.LinesEditor
{
    public class Model
    {
        public readonly Walls Walls = new Walls ();
        public readonly Dictionary<WallData, GameObject> WallGameObjects = new Dictionary<WallData, GameObject> ();
    }
}