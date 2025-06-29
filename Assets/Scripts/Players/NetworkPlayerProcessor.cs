using System.Collections.Generic;
using System.Linq;
using Scripts.Events;
using Scripts.Gesture_Editor_SDK.Realtime;
using Scripts.Gestures;
using Scripts.Static;
using Scripts.Static.Definitions;
using Scripts.Systems;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.PlayerLogic
{
    [RequireComponent(typeof(Player))]
    public class NetworkPlayerProcessor : NetworkBehaviour
    {
        private Player  _player;

        private bool _isSynchronized;

        public PlayerData data => _player.data;
        public UnityEvent onPoolPrefabs = new UpdateEvent();

        private void Awake()
        {
            _player = GetComponent<Player>();
        }

        public override void OnNetworkSpawn()
        {
            Debug.Log("NETWORK SPAWN");

            var IsPlayer = IsClient || IsHost;
            transform.name = $"Player {OwnerClientId}";

            if (IsServer)
            {
                data.onComponentsInitialized.AddListener(PoolPrefabsServerRpc);
            }

            if (IsPlayer && !IsOwner)
            {
                _player.SetEnemy(OwnerClientId);
            }

            if (IsPlayer && IsOwner)
            {
                _player.SetOwner(OwnerClientId);
                _player.data.rig.anchors.Body.position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                Recognizer.onSharedFrameBetweenDevices.AddListener((frame) =>
                {
                    OnLocalClientFrameRecognizedServerRpc(frame, OwnerClientId);
                });
            }

            if (IsServer && !IsHost)
            {
                _player.SetEnemy(OwnerClientId);
                _player.data.characterController.SetAvatarType(AvatarType.None);
            }
        }
        [ServerRpc]
        public void PoolPrefabsServerRpc()
        {
            // spawn characters
            data.characterController.SpawnCharacters();
            // spawn abilities
            data.abilityController.SpawnWeapons(data.characterController.characterConfigs, this.transform);
            
            Dictionary<string, ulong[]> dict = new();
            foreach (var key in data.abilityController.abilitiesLib.characterAbilities.Keys)
            {
                var names = data.abilityController.abilitiesLib.characterAbilities[key].Keys.ToArray();
                ulong[] ids= new ulong[names.Length];
            
                for(int i = 0; i < names.Length; i ++)
                {
                    if(data.abilityController.abilitiesLib.characterAbilities[key][names[i]].TryGetNetcodeId(out ulong id))
                        ids[i] = id;
                }
                dict.Add(key, ids);
            }
            // say client to spawn characters and abilities
            PoolPrefabsClientRpc(JsonUtility.ToJson(dict));
            UpdateCharacterServerRpc(_player.debugCharacter.ToString());
            onPoolPrefabs?.Invoke();
            
        }
        [ClientRpc] public void PoolPrefabsClientRpc(string weapons)
        {
            if (!IsServer)
            {
                data.characterController.SpawnCharacters();
                _player.data.abilityController.SetSpawnedWeapons(JsonUtility.FromJson<Dictionary<string, ulong[]>>(weapons));
                onPoolPrefabs?.Invoke();
            }

            _player.isInitialized = true;
        }

        [ServerRpc]
        public void OnLocalClientFrameRecognizedServerRpc(string frameName, ulong client)
        {
            if (!IsOwner)
            {
                Debug.Log("Play Gesture Frame of player " + _player.name);
                _player.data.abilityController.SimulateFrame(_player.data.hands, frameName);
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
                _player.data.abilityController.SimulateFrame(_player.data.hands, frameName);
            }
        }
        [ServerRpc]
        public void UpdateCharacterServerRpc(string characterName)
        {
            data.characterController.SetCharacter(characterName);
            data.abilityController.AddCharacterToInventory(characterName);
            UpdateCharacterClientRpc(characterName);
        }
        [ClientRpc]
        public void UpdateCharacterClientRpc(string characterName)
        {
            if (!IsServer)
            {
                data.characterController.SetCharacter(characterName);
                data.abilityController.AddCharacterToInventory(characterName);
            }
        }
    }
}