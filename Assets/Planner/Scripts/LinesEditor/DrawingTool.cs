using System.Linq;
using Planner.Geometry;
using Planner.Input;
using Planner.Model;
using Planner.RoomGeometry;
using Planner.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Planner.LinesEditor
{
    public class DrawingTool : MonoBehaviour
    {
        enum WallIntersectionType
        {
            NoIntersection,
            Intersection,
            PointOnWall
        }

        [SerializeField]
        private Dot dotPrefab;

        [SerializeField]
        private float planeDistance;

        [SerializeField]
        private Dot cursorDot;

        [SerializeField]
        private CursorLine cursorLine;

        [Inject (Id = ID.MainCamera)]
        private Camera mainCamera;

        [Inject]
        private InputTrigger<MouseMovedTrigger, object, MouseMovedEventArgs>.Factory mouseMovedTriggerFactory;

        [Inject]
        private InputTrigger<MouseButtonUpTrigger, int, MouseEventArgs>.Factory mouseButtonUpTriggerFactory;

        [Inject]
        private Model model;

        private MouseMovedTrigger mouseMovedTrigger;
        private MouseButtonUpTrigger mouseButtonUpTrigger;
        private Plane dotsPlane;
        private Plane cursorPlane;
        private Dot lastDot;

        private void Awake ()
        {
            mouseMovedTrigger = mouseMovedTriggerFactory.Create ();
            mouseButtonUpTrigger = mouseButtonUpTriggerFactory.Create ();

            dotsPlane = new Plane (Vector3.forward, new Vector3 (0f, 0f, planeDistance));
            cursorPlane = new Plane (Vector3.forward, new Vector3 (0f, 0f, planeDistance - 1));
        }

        private void OnEnable ()
        {
            mouseMovedTrigger.Arm ();
            mouseButtonUpTrigger.Arm ();
            mouseMovedTrigger.Fired += MouseMovedTriggerOnFired;
            mouseButtonUpTrigger.Fired += MouseButtonUpTriggerOnFired;
            cursorDot.Activate ();
            cursorLine.Activate ();
        }

        private void OnDisable ()
        {
            mouseMovedTrigger.Disarm ();
            mouseButtonUpTrigger.Disarm ();
            mouseMovedTrigger.Fired -= MouseMovedTriggerOnFired;
            mouseButtonUpTrigger.Fired -= MouseButtonUpTriggerOnFired;
            cursorDot.Deactivate ();
            cursorLine.Deactivate ();
        }

        private Dot GetTappedDot (Ray ray)
        {
            RaycastHit hit;
            if (!Physics.Raycast (ray, out hit, float.MaxValue, Layer.Mask.Dots))
                return null;

            return hit
                .collider
                .GetComponentInParent<Dot> ();
        }

        private Dot CreateNewDot (Ray ray)
        {
            float enter;
            Assert.IsTrue (dotsPlane.Raycast (ray, out enter));
            Vector3 hitPoint = ray.GetPoint (enter);

            var dot = Instantiate (dotPrefab);
            dot.transform.position = hitPoint;

            return dot;
        }

        private WallIntersectionType CheckNewWallIntersections (
            Vector2 start,
            Vector2 end,
            out WallData intersectedWall,
            out Vector2 intersection)
        {
            var line = Line.Create (start, end);
            foreach (var wall in model.Walls.AllWalls) {
                WallSegmentBounds wallSegmentBounds;
                var intersectsWithWall = wall.Intersects (line, out intersection, out wallSegmentBounds) &&
                                         intersection.LayingBetween (wall.Start, wall.End);
                if (!intersectsWithWall)
                    continue;

                var intersectionInsideNewWall = intersection.LayingBetween (start, end);
                var a = wallSegmentBounds.Inner.Calculate (end);
                var b = wallSegmentBounds.Outer.Calculate (end);
                var intersectionOnWall = a * b <= 0;

                if (intersectionInsideNewWall) {
                    intersectedWall = wall;
                    return WallIntersectionType.Intersection;
                }

                if (intersectionOnWall) {
                    Debug.Log ($"a {a} b {b} {wall}");
                    intersectedWall = wall;
                    return WallIntersectionType.PointOnWall;
                }
            }

            intersectedWall = default (WallData);
            intersection = default (Vector2);
            return WallIntersectionType.NoIntersection;
        }

        private WallIntersectionType CheckNewWallIntersections (
            out WallData intersectedWall,
            out Vector2 intersection)
        {
            return CheckNewWallIntersections (
                lastDot.transform.position,
                cursorDot.transform.position,
                out intersectedWall,
                out intersection);
        }

        private WallIntersectionType CheckNewWallIntersections ()
        {
            WallData wallData;
            Vector2 intersection;
            return CheckNewWallIntersections (out wallData, out intersection);
        }

        private void DestroyWallGameObject (WallData wall)
        {
            GameObject go;
            var gameObjects = model.WallGameObjects;
            if (!gameObjects.TryGetValue (wall, out go))
                return;

            gameObjects.Remove (wall);
            Destroy (go);
        }

        private void CreateWallGameObject (WallData wall)
        {
            GameObject go = RoomMeshesGenerator.CreateWallMeshes (
                wall,
                model.Walls,
                WallMeshesGenerator.Options.Top | WallMeshesGenerator.Options.TopDismissHeight);

            go.transform.Rotate (Vector3.right, -90f);
            go.transform.Translate (0f, 0f, planeDistance + 1, Space.World);

            model
                .WallGameObjects
                .Add (wall, go);
        }

        private void MouseButtonUpTriggerOnFired (object sender, MouseEventArgs e)
        {
            var ray = mainCamera.ScreenPointToRay (e.Position);
            Dot tappedDot = GetTappedDot (ray);
            bool createNewDot = tappedDot == null;

            if (lastDot == null) {
                if (createNewDot)
                    lastDot = CreateNewDot (ray);

                return;
            }

            WallData intersectedWall;
            Vector2 intersection;
            var intersectionType = CheckNewWallIntersections (out intersectedWall, out intersection);
            if (intersectionType == WallIntersectionType.Intersection)
                return;

            var newDot = createNewDot
                ? CreateNewDot (ray)
                : tappedDot;

            var walls = model.Walls;
            var wallEnd = newDot.transform.position;

            Debug.Log ($"intersectionType {intersectionType}");
            if (intersectionType == WallIntersectionType.PointOnWall) {
                wallEnd = intersection;
                newDot.transform.position = intersection.ToVector3 (planeDistance);

                walls.RemoveWallData (intersectedWall);
                DestroyWallGameObject (intersectedWall);

                var newWallA = WallData.CreateStreight (
                    intersectedWall.Start,
                    wallEnd,
                    intersectedWall.Width,
                    intersectedWall.Height);

                var newWallB = WallData.CreateStreight (
                    wallEnd,
                    intersectedWall.End,
                    intersectedWall.Width,
                    intersectedWall.Height);

                walls.AddWallData (newWallA);
                walls.AddWallData (newWallB);
                CreateWallGameObject (newWallA);
                CreateWallGameObject (newWallB);
            }

            var newWall = WallData.CreateStreight (lastDot.transform.position, wallEnd, 0.75f, 2f);
            var neighbours = walls
                .GetPointWalls (newWall.Start)
                .ToList ();
            neighbours.AddRange (walls.GetPointWalls (newWall.End));

            walls.AddWallData (newWall);
            neighbours.Add (newWall);

            foreach (var neighbour in neighbours) {
                DestroyWallGameObject (neighbour);
                CreateWallGameObject (neighbour);
            }

            lastDot = newDot;
        }

        private void MouseMovedTriggerOnFired (object sender, MouseMovedEventArgs e)
        {
            var ray = mainCamera.ScreenPointToRay (e.Position);
            float enter;
            Assert.IsTrue (cursorPlane.Raycast (ray, out enter));

            Vector3 hitPoint = ray.GetPoint (enter);
            cursorDot.transform.position = hitPoint;

            bool hasLastDot = lastDot != null;
            cursorLine.SetActive (lastDot);
            if (hasLastDot) {
                cursorLine.Refresh (lastDot, cursorDot);
                cursorLine.SetPossible (CheckNewWallIntersections () != WallIntersectionType.Intersection);
            }
        }
    }
}