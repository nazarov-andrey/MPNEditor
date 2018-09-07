using Planner.Model;
using UnityEngine;
using SystemVector2 = System.Numerics.Vector2;

namespace Planner.Geometry
{
    public static class Vector2Extensions
    {
        public static Vector2 GetNormalVector (this Vector2 vector)
        {
            return new Vector2 (vector.y, -vector.x);
        }

        public static Vector2 TransposePoint (this Vector2 point, Vector2 direction, float distance)
        {
            return point + direction.normalized * distance;
        }

        public static SystemVector2 ToSystemVector2 (this Vector2 vector)
        {
            return new SystemVector2 (vector.x, vector.y);
        }

        public static Vector3 ToVector3 (this Vector2 vector, float z = 0f, bool flipYZ = false)
        {
            var result = new Vector3 (vector.x, vector.y, z);
            if (flipYZ)
                result = result.FlipYZ ();

            return result;
        }

        public static float GetWallAngle (this Vector2 vector, WallData wall)
        {
            var angle = Vector2.SignedAngle (vector, wall.GetVector ());
            if (angle < 0)
                angle += 360f;
            return angle;
        }

        public static bool LayingBetween (this Vector2 point, Vector2 pointA, Vector2 pointB)
        {
            return point != pointA &&
                   point != pointB &&
                   (point.x - pointA.x) * (point.x - pointB.x) < 0 &&
                   (point.y - pointA.y) * (point.y - pointB.y) < 0;
        }
    }
}