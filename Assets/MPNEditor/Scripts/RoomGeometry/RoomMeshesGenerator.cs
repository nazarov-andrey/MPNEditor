using System;
using System.Linq;
using Boo.Lang;
using Geometry;
using Model;
using UnityEngine;

namespace RoomGeometry
{
    public static class RoomMeshesGenerator
    {
        public static GameObject CreateWallMeshes (
            WallData wall,
            WallData nextWall,
            WallData prevWall,
            WallMeshesGenerator.Options options = WallMeshesGenerator.Options.Default,
            Transform parent = null)
        {
            return CreateWallMeshes (wall, nextWall, nextWall, prevWall, prevWall, options, parent);
        }

        public static GameObject CreateWallMeshes (
            WallData wall,
            WallData nextInnerWall,
            WallData nextOuterWall,
            WallData prevInnerWall,
            WallData prevOuterWall,
            WallMeshesGenerator.Options options = WallMeshesGenerator.Options.Default,
            Transform parent = null)
        {
            var meshes = WallMeshesGenerator.GetWallMeshes (
                prevInnerWall,
                prevOuterWall,
                wall,
                nextInnerWall,
                nextOuterWall,
                options);

            var wallTransform = new GameObject (wall.ToString ()).transform;
            foreach (var mesh in meshes) {
                var meshGameObject = MeshGenerator.CreateGameObject (mesh.name, mesh);
                meshGameObject
                    .transform
                    .SetParent (wallTransform, false);
            }

            wallTransform.SetParent (parent);
            return wallTransform.gameObject;
        }

        public static GameObject CreateWallMeshes (
            WallData wall,
            Walls walls,
            WallMeshesGenerator.Options options = WallMeshesGenerator.Options.Default,
            Transform parent = null)
        {
            var wallInverseVector = wall.GetInverseVector ();
            var prevWalls = walls
                .GetPointWalls (wall.Start)
                .Where (x => x != wall)
                .Select (x => wall.Start == x.End ? x : x.Reverse ())
                .Select (x => new Tuple<WallData, float> (x, wallInverseVector.GetWallAngle (x)))
                .OrderBy (x => x.Item2)
                .ToArray ();
            var nextWalls = walls
                .GetPointWalls (wall.End)
                .Where (x => x != wall)
                .Select (x => wall.End == x.Start ? x : x.Reverse ())
                .Select (x => new Tuple<WallData, float> (x, wallInverseVector.GetWallAngle (x)))
                .OrderBy (x => x.Item2)
                .ToArray ();

            var prevInnerWall = prevWalls.LastOrDefault ()?.Item1;
            var prevOuterWall = prevWalls.FirstOrDefault ()?.Item1;
            var nextInnerWall = nextWalls.FirstOrDefault ()?.Item1;
            var nextOuterWall = nextWalls.LastOrDefault ()?.Item1;

            return CreateWallMeshes (wall, nextInnerWall, nextOuterWall, prevInnerWall, prevOuterWall, options, parent);
        }


        public static void CreateRoomMeshes (
            Walls walls,
            WallMeshesGenerator.Options options = WallMeshesGenerator.Options.Default)
        {
            var rooms = RoomsFinder.FindRooms (walls);
            /*        foreach (var room in rooms) {
                        Debug.Log (
                            $"room {string.Join (",", Array.ConvertAll (room, x => $"[wall {x.Points.First ()} {x.Points.Last ()}]"))}");
                    }*/

            for (int j = 0; j < rooms.Length; j++) {
                var roomTransform = new GameObject ($"Room {j}").transform;
                var room = rooms[j];
                for (int count = room.Length, i = 0; i < count; i++) {
                    var wall = room[i];
                    CreateWallMeshes (wall, walls, options, roomTransform);
                }
            }
        }
    }
}