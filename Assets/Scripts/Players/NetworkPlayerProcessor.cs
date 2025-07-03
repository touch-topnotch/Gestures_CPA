using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Scripts.Events;
using Scripts.GameControllers;
using Scripts.Gestures;
using Scripts.Players;
using Scripts.Static.Definitions;
using Scripts.Weapons;
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

                var spawnPoints = NetworkManager.GetComponent<GameController>().gameProperties.spawnPoints;
                _player.rig.anchors.Root.position = spawnPoints[(int)OwnerClientId%spawnPoints.Length].position;

            }
            
            if (IsServer && !IsHost)
            {
                _player.InitializePlayer(this.NetworkBehaviourId, new PlayerProperties(RigType.NoRig, AvatarType.None, lastPlayerProperties.character));
            }
            
            _player.onPlayerInitialized.AddListener(() =>
            {
                oldPlayer.gameObject.SetActive(false);
            });
            if (IsServer)
            {
                
            }
        }

        [ClientRpc]
        public void SetWeaponsClientRpc(string spawnedWeaponsData)
        {
            if (!IsOwner)
                return;
            var weapons = JsonConvert.DeserializeObject<Dictionary<CharacterType, ulong[]>>(spawnedWeaponsData);
            data.abilityController.SetSpawnedWeapons(weapons);
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
        
        [ClientRpc]
        public void StartUseAbilitiesClientRpc(ushort[] debugCharacterAbilities)
        {
            if (IsOwner)
            {
                Debug.Log(
                    "Самое важное сообщение в твоей жизни [Client rpc] private void StartGameSessionClientRpc(ulong playerId) ");
                data.abilityController
                    .AddCharacterToInventory(data.characterController.currentCharacter.name);

                foreach (var VARIABLE in debugCharacterAbilities)
                {
                    data.abilityController.AddCharacterToInventory(((CharacterType)VARIABLE).ToString());
                }

                data.abilityController.UseCharacterAbilities();
            }
        }
    }
}