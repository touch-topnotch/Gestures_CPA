using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using UnityEngine;

namespace Network
{
    public class RelayCreator : MonoBehaviour
    {
        private const string CONNECTION_TYPE = "udp";

        public async UniTask<string> CreateRelay(int maxConnection)
        {
            try
            {
                var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
                var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                var relayServerData = new RelayServerData(allocation, CONNECTION_TYPE);
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
    }
}