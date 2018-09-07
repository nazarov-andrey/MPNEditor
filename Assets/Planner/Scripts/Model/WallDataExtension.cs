using Planner.Geometry;
using UnityEngine;

namespace Planner.Model
{
    public static class WallDataExtension
    {
        public static Vector2 GetVector (this WallData wall)
        {
            return wall.End - wall.Start;
        }

        public static Vector2 GetInverseVector (this WallData wall)
        {
            return -wall.GetVector ();
        }

        public static bool Intersects (this WallData wall, WallData anotherWall, out Vector2 intersection)
        {
            intersection = default (Vector2);

            Line[] segments = wall.Segments.Value;
            Line[] anotherSegments = wall.Segments.Value;

            for (int i = 0; i < segments.Length; i++) {
                var segment = segments[i];
                for (int j = i; j < anotherSegments.Length; j++) {
                    var anotherSegment = anotherSegments[j];
                    if (segment.Intersects (anotherSegment, out intersection))
                        return true;
                }
            }

            return false;
        }

        public static bool Intersects (
            this WallData wall,
            Line line,
            out Vector2 intersection,
            out WallSegmentBounds wallSegmentBounds)
        {
            intersection = default (Vector2);

            var segments = wall.Segments.Value;
            for (int i = 0; i < segments.Length; i++) {
                if (segments[i].Intersects (line, out intersection)) {
                    wallSegmentBounds = wall.Bounds.Value[i];
                    return true;
                }
            }

            wallSegmentBounds = default (WallSegmentBounds);
            return false;
        }

        public static bool ContainsPoint (this WallData wall, Vector2 point)
        {
            var segments = wall.Segments.Value;
            for (int i = 0; i < segments.Length; i++) {
                if (segments[i].ContainsPoint (point))
                    return true;
            }

            return false;
        }
    }
}