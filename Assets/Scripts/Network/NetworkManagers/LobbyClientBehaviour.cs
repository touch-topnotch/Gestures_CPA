using Scripts.Network.NetworkManagers;
using Scripts.PlayerLogic;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Scripts.Network
{
    public class LobbyClientBehaviour : CustomNetworkBehaviour

    {
        private NetworkUser _ownUser;
        private Player _ownPlayer;

        [Inject]
        private void Construct(Player player)
        {
            Network = GetComponent<NetworkManager>();
            
            _ownPlayer = player;
            Network.OnClientConnectedCallback += ClientConnected;
            Network.StartClient();
        }

        private void ClientConnected(ulong clientId)
        {
            print("Client connected!");
            _ownUser = Network.LocalClient.PlayerObject.GetComponent<NetworkUser>();
            _ownPlayer.ConnectToUser(_ownUser);
  
        }
    }
}