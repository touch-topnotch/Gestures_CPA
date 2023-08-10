using Scripts.Gestures;
using Scripts.Static;
using UnityEngine;
using Zenject;

namespace Scripts.Installers
{
    public class GRecognitionInstaller: MonoInstaller
    {
        [SerializeField] private GameObject recognizerPrefab;
        private GestureCombiner _combiner;
        private Recognizer _recognizer;
        public override void InstallBindings()
        {
            _recognizer = Spawner.SpawnPrefab(recognizerPrefab, Container).GetComponent<Recognizer>();
            Container.Bind<Recognizer>().FromInstance(_recognizer).AsSingle();
            Container.Bind<GestureCombiner>().FromNew().AsSingle();
        }
    }
}