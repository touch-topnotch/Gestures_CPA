using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Network
{
    public class RelayJoiner : MonoBehaviour
    {
        private const string CONNECTION_TYPE = "udp";
        private const string RELAY_CODE_KEY =  "RelayCode"; 
        
        public async UniTask AutojoinRelayWithLobby(string lobbyID)
        {
            while (true)
            {
                var lobby = await LobbyService.Instance.GetLobbyAsync(lobbyID);
                if (lobby.Data != null)
                {
                    if (lobby.Data.TryGetValue(RELAY_CODE_KEY, out var code))
                    {
                        await JoinRelay(code.Value);
                        return;
                    }
                }
                await UniTask.Delay(2000);
            }
        }

        public async UniTask JoinRelay(string joinCode)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                var relayServerData = new RelayServerData(joinAllocation, CONNECTION_TYPE);
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                    relayServerData
                );

                NetworkManager.Singleton.StartClient();
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
            }
        }
    }
}