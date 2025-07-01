using System.Collections.Generic;
using System.Linq;
using Scripts.Events;
using Scripts.GameControllers;
using Scripts.Gestures;
using Scripts.Players;
using Scripts.Static.Definitions;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
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
            var oldPlayer = NetworkManager.GetComponent<GameController>().gameProperties.player;
            

            PlayerProperties lastPlayerProperties = oldPlayer ? oldPlayer.playerProperties
                : new PlayerProperties(RigType.NoRig, AvatarType.Enemy, CharacterType.Anger);

            var IsPlayer = IsClient || IsHost;
            transform.name = $"Player {OwnerClientId}";

          
            if (IsPlayer && !IsOwner)
            {
                _player.InitializePlayer(this.NetworkBehaviourId, new PlayerProperties(RigType.NoRig, AvatarType.Enemy, lastPlayerProperties.character));
            }

            if (IsPlayer && IsOwner)
            {
                _player.InitializePlayer(this.OwnerClientId, new PlayerProperties(lastPlayerProperties.rig, AvatarType.Local, lastPlayerProperties.character));
                
                _player.onPlayerInitialized.AddListener(() =>
                {
                    PlayerData.local = _player.data;
                });
                _player.onPlayerInitialized.AddListener(_player.CreateRecognizer);
                Recognizer.onSharedFrameBetweenDevices.AddListener((frame) =>
                {
                    OnLocalClientFrameRecognizedServerRpc(frame, OwnerClientId);
                });

                _player.rig.anchors.Root.position =
                    NetworkManager.GetComponent<GameController>().gameProperties.spawnPoints[OwnerClientId].position;

            }
            
            if (IsServer && !IsHost)
            {
                _player.InitializePlayer(this.NetworkBehaviourId, new PlayerProperties(RigType.NoRig, AvatarType.None, lastPlayerProperties.character));
            }
            if (IsServer)
            {
                _player.onPlayerInitialized.AddListener(PoolNetworkPrefabsServerRpc);
            }
            oldPlayer.gameObject.SetActive(false);
        }
        [ServerRpc]
        public void PoolNetworkPrefabsServerRpc()
        {
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
            PoolNetworkPrefabsClientRpc(JsonUtility.ToJson(dict));
            onPoolPrefabs?.Invoke();
            
        }
        [ClientRpc] public void PoolNetworkPrefabsClientRpc(string weapons)
        {
            if (!IsServer)
            {
                _player.data.abilityController.SetSpawnedWeapons(JsonUtility.FromJson<Dictionary<string, ulong[]>>(weapons));
                onPoolPrefabs?.Invoke();
            }
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