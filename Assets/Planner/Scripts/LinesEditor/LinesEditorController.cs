using Planner.RoomGeometry;
using UnityEngine;
using Zenject;

namespace Planner.LinesEditor
{
    public class LinesEditorController : MonoBehaviour
    {
        [Inject]
        private Model model;

        private void Update ()
        {
            if (UnityEngine.Input.GetKeyUp (KeyCode.Space))
                RoomMeshesGenerator.CreateRoomMeshes (model.Walls);
        }
    }
}