using Scripts.Events;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;
namespace Scripts.Installers
{
    
    public class EventInstaller: MonoInstaller
    {
        public UpdateEvent OnFrameUpdated = new UpdateEvent();

        public override void InstallBindings()
        {
            Container.Bind<UpdateEvent>().FromInstance(OnFrameUpdated).AsSingle().NonLazy();
            Debug.Log("Events has initialized");
        }
        private void Update()
        {
            
            OnFrameUpdated?.Invoke();
        }
    }
}