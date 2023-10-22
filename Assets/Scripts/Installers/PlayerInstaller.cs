using Scripts.PlayerLogic;
using Scripts.Static;
using UnityEngine;
using Zenject;

namespace Scripts.Installers
{
    public class PlayerInstaller: MonoInstaller
    {
        private GameObject _playerPrefab;
        private Player _xrPlayer;
        
        
        // lets spawn player at first and after connection add him to NetworkUser
        public override void InstallBindings()
        {
            
            SetPlayerByPlatform();
            _xrPlayer = SpawnPlayer(Container); // ???
            Container.Bind<Player>().FromInstance(_xrPlayer).AsSingle().NonLazy();
            DontDestroyOnLoad(_xrPlayer.gameObject);
            //_xrPlayer.Initialize();
        }

        private Player SpawnPlayer(in DiContainer container)
        {
            return Spawner.SpawnInjectedPrefab(_playerPrefab, container).GetComponent<Player>();
        }

        private void SetPlayerByPlatform()
        {
            _playerPrefab = Resources.Load("Players/Player") as GameObject;
            // if (XRGeneralSettings.Instance?.Manager?.activeLoader)
            // {
            //     
            // }
            // else
            // {
            //     _playerPrefab = Resources.Load("Players/PC Player") as GameObject;
            // }
        }
    }
}
// we have a SpawnerNetUser script, which create any users (our or not, doesn't matter)
// in playerInstaller we should give our NetworkSpawner component