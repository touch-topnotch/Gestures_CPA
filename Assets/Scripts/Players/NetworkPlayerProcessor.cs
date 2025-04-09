using System.Collections.Generic;
using Scripts.GameControllers;
using Scripts.Gestures;
using Scripts.Network;
using Scripts.Static;
using Sirenix.OdinInspector;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using ClientTransform = Scripts.Network.ClientTransform;

namespace Scripts.PlayerLogic
{
 

    [RequireComponent(typeof(Player))]
    public class NetworkPlayerProcessor : NetworkBehaviour
    {

        private Player _player;
        
        private bool _isSynchronized;

        public Player localPlayer => _player;


        private void Awake()
        {
            _player = GetComponent<Player>();
        }
        
        public override void OnNetworkSpawn()
        {
        
            
            Debug.Log("NETWORK SPAWN");
            transform.name = $"Player {OwnerClientId}";
            
            
            
            _player.characterPool.SetMaterialId((int)OwnerClientId); 
            _player.characterPool.SetCharacter((int)OwnerClientId % 2 == 0 ? "Anger" : "Grief");
            
            if ((IsClient || IsHost ) && !IsOwner)
            { 
                _player.rigType = RigType.NoRig;
                _player.characterPool.SetAvatarType(AvatarType.Enemy);

            }

            if (IsOwner && (IsHost || IsClient))
            {
                _player.rigType = RigType.PCRig;
                _player.InitializeLocally(); 
                
                _player.characterPool.SetAvatarType(AvatarType.Local);
                _player.curRig.Anchors.Body.position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
             
                _player.gestureCombiner.OnFrameRecognized.AddListener(
                    (frame) => { OnLocalClientFrameRecognizedServerRpc(frame, OwnerClientId); });
            }
            
            if (IsServer && !IsHost)
            {
                _player.rigType = RigType.NoRig;
                _player.characterPool.SetAvatarType(AvatarType.None);
            }
        }
        
        [ServerRpc]
        public void OnLocalClientFrameRecognizedServerRpc(string frameName, ulong client)
        {
            Debug.Log("Play Gesture Frame of player "+ _player.name);
            _player.gestureCombiner.SimulateFrame(frameName);
            CallFrameRecognizedClientRpc(frameName, client);
        }

        // void called on all clients on Player[Client]
        [ClientRpc]
        void CallFrameRecognizedClientRpc(string frameName, ulong client)
        {
            Debug.Log("void CallFrameRecognizedClientRpc(string frameName, ulong client)");
            if (OwnerClientId == client && !IsOwner)
            {
                Debug.Log("Play Gesture Frame of player " + _player.name);
                _player.gestureCombiner.SimulateFrame(frameName);
            }
        }
    }
}
