using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using MPNEditor.Utils;
using RoomGeometry;
using TouchScript.Gestures;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Zenject;

namespace MPNEditor.LinesEditor
{
    public class LinesEditorController : MonoBehaviour
    {
        [SerializeField]
        private Dot dotPrefab;

        [SerializeField]
        private float planeDistance;

        [SerializeField]
        private Dot cursorDot;

        [SerializeField]
        private CursorLine cursorLine;

        [SerializeField]
        private Button button;

        [Inject (Id = ID.MainCamera)]
        private Camera mainCamera;

        [Inject (Id = ID.FullscreenTap)]
        private TapGesture fullscreenTap;

        private Plane dotsPlane;
        private Plane cursorPlane;
        private Dot lastDot;
        private bool placeDotOnTap;

        private Walls walls = new Walls ();
        private Dictionary<WallData, GameObject> wallGameObjects = new Dictionary<WallData, GameObject> ();

        private void Awake ()
        {
            dotsPlane = new Plane (Vector3.forward, new Vector3 (0f, 0f, planeDistance));
            cursorPlane = new Plane (Vector3.forward, new Vector3 (0f, 0f, planeDistance - 1));

            cursorDot.name = "CursorDot";
            cursorDot.Deactivate ();
            cursorLine.Deactivate ();
        }

        private void OnEnable ()
        {
            fullscreenTap.Tapped += FullscreenTapOnTapped;
            button.onClick.AddListener (ButtonClickListener);
        }

        private void OnDisable ()
        {
            fullscreenTap.Tapped -= FullscreenTapOnTapped;
            button.onClick.RemoveListener (ButtonClickListener);
        }

        private void Update ()
        {
            placeDotOnTap = IsDotsPlacingKeyPressed ();

            cursorDot.SetActive (placeDotOnTap);
            cursorLine.SetActive (placeDotOnTap);

            if (!placeDotOnTap) {
                lastDot = null;
                return;
            }

            var ray = mainCamera.ScreenPointToRay (Input.mousePosition);
            float enter;
            Assert.IsTrue (cursorPlane.Raycast (ray, out enter));

            Vector3 hitPoint = ray.GetPoint (enter);
            cursorDot.transform.position = hitPoint;

            bool hasLastDot = lastDot != null;
            cursorLine.SetActive (lastDot);
            if (hasLastDot) {
                cursorLine.Refresh (lastDot, cursorDot);
                cursorLine.SetPossible (true);
            }
        }

        private bool IsDotsPlacingKeyPressed ()
        {
            return Input.GetKey (KeyCode.LeftControl);
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

        private void ButtonClickListener ()
        {
            RoomMeshesGenerator.CreateRoomMeshes (walls);
        }

        private void FullscreenTapOnTapped (object sender, EventArgs e)
        {
            if (!placeDotOnTap)
                return;

            var ray = mainCamera.ScreenPointToRay (fullscreenTap.ScreenPosition);

            Dot newDot = GetTappedDot (ray);
            if (newDot == null) {
                float enter;
                Assert.IsTrue (dotsPlane.Raycast (ray, out enter));
                Vector3 hitPoint = ray.GetPoint (enter);

                newDot = Instantiate (dotPrefab);
                newDot.transform.position = hitPoint;
            }

            if (lastDot != null) {
                var wall = WallData.CreateStreight (lastDot.transform.position, newDot.transform.position, 0.75f, 2f);

                var neighbours = walls
                    .GetPointWalls (wall.Start)
                    .ToList ();
                neighbours.AddRange (walls.GetPointWalls (wall.End));

                walls.AddWallData (wall);
                neighbours.Add (wall);

                foreach (var neighbour in neighbours) {
                    GameObject go;
                    if (wallGameObjects.TryGetValue (neighbour, out go)) {
                        wallGameObjects.Remove (neighbour);
                        Destroy (go);
                    }

                    go = RoomMeshesGenerator.CreateWallMeshes (
                        neighbour,
                        walls,
                        WallMeshesGenerator.Options.Top | WallMeshesGenerator.Options.TopDismissHeight);

                    go.transform.Rotate (Vector3.right, -90f);
                    go.transform.Translate (0f, 0f, planeDistance + 1, Space.World);
                    wallGameObjects.Add (neighbour, go);
                }
            }

            lastDot = newDot;
        }
    }
}