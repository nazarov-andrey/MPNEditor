using Planner.Input;
using UnityEngine;
using Zenject;

namespace Planner
{
    public class RootInstaller : MonoInstaller
    {
        [SerializeField]
        private Camera mainCamera;

        public override void InstallBindings ()
        {
            Container
                .BindInstance (mainCamera)
                .WithId (ID.MainCamera);

            Container
                .Bind (
                    x => x
                        .AllNonAbstractClasses ()
                        .DerivingFrom<IInputTriggerFactory> ())
                .AsSingle ();

            Container
                .Bind<LinesEditor.Model> ()
                .AsSingle ();
        }
    }
}