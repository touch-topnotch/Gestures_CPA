using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Network
{
    public class RelayConnector : MonoBehaviour
    {
        public async Task<string> CreateRelay(int maxConnection)
        {
            try
            {
                var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

                var relayServerData = new RelayServerData(allocation, "udp");

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                    relayServerData
                );
                NetworkManager.Singleton.StartHost();
                return joinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
                return e.Message;
            }
        }


        public async void JoinRelay(string joinCode)
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                var relayServerData = new RelayServerData(joinAllocation, "udp");
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