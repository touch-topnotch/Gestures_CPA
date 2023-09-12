using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Scripts.Installers
{
    public class NetworkInstaller: MonoInstaller
    {
        [SerializeField] private NetworkManager _networkManager;
        [SerializeField] private DiContainer _diContainer;
        public override void InstallBindings()
        {
            _diContainer = Container;
            _diContainer.Bind<NetworkManager>().FromInstance(_networkManager);

        }
    }
}