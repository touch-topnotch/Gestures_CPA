using Unity.Netcode;
using UnityEngine;

namespace Scripts.Network.NetworkManagers
{
    public abstract class CustomNetworkBehaviour: MonoBehaviour
    {
        protected NetworkManager Network;
        protected virtual void Awake()
        {
            Network = GetComponent<NetworkManager>();
        }
    }
}