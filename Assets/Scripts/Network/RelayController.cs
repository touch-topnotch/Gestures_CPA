using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Network
{
    public class RelayController : MonoBehaviour
    {
        [SerializeField] private LobbyConnector lobbyConnector;
        [SerializeField] private RelayJoiner relayJoiner;
        [SerializeField] private RelayCreator relayCreator;
    
        private const int MAX_PLAYERS = 4;
    
        private void OnEnable()
        {
            lobbyConnector.LobbyConnected += OnLobbyConnected;
            lobbyConnector.LobbyConnectedAsHost += OnLobbyConnectedAsHost;
        }

        private void OnDisable()
        {
            lobbyConnector.LobbyConnected -= OnLobbyConnected;
            lobbyConnector.LobbyConnectedAsHost -= OnLobbyConnectedAsHost;
        }
    
        private async void OnLobbyConnected(string obj)
        {
            await UniTask.SwitchToMainThread();
            await relayJoiner.AutojoinRelayWithLobby(obj);
        }
    
        private async void OnLobbyConnectedAsHost(string obj)
        {
            await UniTask.SwitchToMainThread();
            var code = await relayCreator.CreateRelay(MAX_PLAYERS);

            lobbyConnector.SetCodeAsRelayCode(code);
        }
    }
}
