using Scripts.Abilities;
using Scripts.Gestures;
using UnityEngine;
using Zenject;

namespace Scripts.Installers
{
    public class GRecognitionInstaller : MonoInstaller
    {
        [SerializeField] private Recognizer recognizer;
        private AbilityController _abilityController;

        public override void InstallBindings()
        {
            Container.Bind<Recognizer>().FromInstance(recognizer).AsSingle();
            Container.Bind<AbilityController>().FromNew().AsSingle();
        }
    }
}