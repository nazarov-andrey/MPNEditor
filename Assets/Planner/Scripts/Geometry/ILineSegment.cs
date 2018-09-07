using System.Collections.Generic;
using UnityEngine;

namespace Planner.Geometry
{
    public class Points : List<Vector2>
    {
    }
    
    public interface ILineSegment
    {
        Points Points { get; }
    }
}