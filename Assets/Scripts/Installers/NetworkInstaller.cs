using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Scripts.Installers
{
    public class NetworkInstaller: MonoInstaller
    {
        [SerializeField] private NetworkManager _networkManager;
        public override void InstallBindings()
        {
            Container.Bind<NetworkManager>().FromInstance(_networkManager);
        }
    }
}