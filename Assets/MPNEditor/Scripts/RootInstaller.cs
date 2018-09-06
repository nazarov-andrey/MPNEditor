using TouchScript.Gestures;
using UnityEngine;
using Zenject;

namespace MPNEditor
{
    public class RootInstaller : MonoInstaller
    {
        [SerializeField]
        private Camera mainCamera;
        
        [SerializeField]
        private TapGesture fullscreenTap;

        public override void InstallBindings ()
        {
            Container
                .BindInstance (mainCamera)
                .WithId (ID.MainCamera);
            
            Container
                .BindInstance (fullscreenTap)
                .WithId (ID.FullscreenTap);
        }
    }
}