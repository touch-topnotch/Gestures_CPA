using Scripts.Gestures;
using Scripts.Hands;
using Scripts.PlayerLogic;
using Scripts.Static;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

namespace Scripts.Installers
{
    public class PlayerInstaller: MonoInstaller
    {
        
        [SerializeField]
        protected GameObject playerPrefab;

        [SerializeField] private Transform spawnPoint;
        [Inject] private DiContainer _diContainer;
        
        private Player _xrPlayer;
        public override void InstallBindings()
        { 
            _xrPlayer = Spawner.SpawnPrefab(playerPrefab, Container, spawnPoint).GetComponent<Player>();
            Container.Bind<Player>().FromInstance(_xrPlayer).AsSingle().NonLazy();
            Container.Bind<RuntimeXRInteractor>().FromInstance(_xrPlayer.xrInteractor).AsSingle().NonLazy();
            Container.Bind<OnGameStateChanged>().FromInstance(_xrPlayer.GameStateChanged).AsSingle().NonLazy();
            Container.Bind<UserHands>().FromInstance(_xrPlayer.playerHands).AsSingle().NonLazy();
            _xrPlayer.Initialize();
        }
    }
}