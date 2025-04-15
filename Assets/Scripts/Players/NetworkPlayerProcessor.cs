using System.Collections.Generic;
using Scripts.GameControllers;
using Scripts.Gestures;
using Scripts.Network;
using Scripts.Static;
using Scripts.Weapons;
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

            var IsPlayer = IsClient || IsHost;
            transform.name = $"Player {OwnerClientId}";
            
            _player.characterPool.SpawnCharacters();
            if (IsServer)
                OnWeaponsInitializedClientRpc(JsonUtility.ToJson(_player.characterPool.SpawnWeapons()));
            if (IsPlayer && !IsOwner)
            {   
                _player.SetEnemy(OwnerClientId);
            }
            if (IsPlayer && IsOwner)
            {
                _player.SetOwner(OwnerClientId);
                _player.curRig.Anchors.Body.position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                _player.gestureCombiner.OnFrameRecognized.AddListener(
                    (frame) => { OnLocalClientFrameRecognizedServerRpc(frame, OwnerClientId); });
            }
            
            if (IsServer && !IsHost)
            {
                _player.SetEnemy(OwnerClientId);
                _player.characterPool.SetAvatarType(AvatarType.None);
            }

          
        }

        [ClientRpc]
        public void OnWeaponsInitializedClientRpc(string weapons)
        {
            if(!IsOwner)
                _player.characterPool.SetWeapons(JsonUtility.FromJson<List<KeyValuePair<string, List<ulong>>>>(weapons));
        }
        [ServerRpc]
        public void OnLocalClientFrameRecognizedServerRpc(string frameName, ulong client)
        {
           
            if (!IsOwner)
            { 
                Debug.Log("Play Gesture Frame of player "+ _player.name);
                _player.gestureCombiner.SimulateFrame(_player.data.hands, frameName);
            }
            CallFrameRecognizedClientRpc(frameName, client);
        }

        // void called on all clients on Player[Client]
        [ClientRpc]
        void CallFrameRecognizedClientRpc(string frameName, ulong client)
        {
            //            Debug.Log($"void CallFrameRecognizedClientRpc(string {frameName}, ulong {client})");
            
            if (OwnerClientId == client && !IsOwner)
            {
                Debug.Log("Play Gesture Frame of player " + _player.name);
                _player.gestureCombiner.SimulateFrame(_player.data.hands, frameName);
            }
        }
    }
}
