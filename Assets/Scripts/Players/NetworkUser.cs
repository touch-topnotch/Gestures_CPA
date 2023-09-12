using Scripts.Hands;
using Scripts.Network;
using Scripts.Static;
using Unity.Netcode;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkUser : NetworkBehaviour
    {
        public SupportHandCreator handsVisualiser;
        public Parenter parenter;
        [SerializeField] private ClientTransform headAnchor;
        
        [HideInInspector] public NetworkObject networkObject;
        
        protected virtual void Awake()
        {
            networkObject = GetComponent<NetworkObject>();

        }
  
      
    }
}