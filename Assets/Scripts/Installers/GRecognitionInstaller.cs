using Scripts.Gestures;
using UnityEngine;
using Zenject;

namespace Scripts.Installers
{
    public class GRecognitionInstaller : MonoInstaller
    {
        [SerializeField] private Recognizer recognizer;
        private GestureCombiner _combiner;

        public override void InstallBindings()
        {
            Container.Bind<Recognizer>().FromInstance(recognizer).AsSingle();
            Container.Bind<GestureCombiner>().FromNew().AsSingle();
        }
    }
}