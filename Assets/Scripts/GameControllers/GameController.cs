using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Scripts.Events;
using Scripts.Network;
using Scripts.PlayerLogic;
using Scripts.Players;
using Scripts.Static;
using Scripts.Static.Definitions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

#if DEDICATED_SERVER
using Unity.Netcode.Transports.UTP;
using Unity.Services.Multiplay;
#endif

namespace Scripts.GameControllers
{
    [RequireComponent(typeof(GameProperties))]
    public class GameController : NetworkManager
    {
        public const int targetFPS = 60;

        private static GameController _gameController;
        public static GameController Instance => _gameController;

        private readonly Dictionary<ulong, NetworkPlayerProcessor>
            _playersDict = new Dictionary<ulong, NetworkPlayerProcessor>();

        [SerializeField] private GameObject ServerInputSystem;
        public Dictionary<ulong, NetworkPlayerProcessor> PlayersDict => _playersDict;
        public GameProperties gameProperties;
        public UnityEvent onPoolPrefabs = new UnityEvent();


#if DEDICATED_SERVER
        private IServerQueryHandler _serverQueryHandler;
        private async void ListenServerEvents()
        {

                    Debug.Log("Unity Services initialized");

                MultiplayEventCallbacks multiplayEventCallbacks = new MultiplayEventCallbacks();
                multiplayEventCallbacks.Allocate += MultiplayEventCallbacks_Allocate;
                multiplayEventCallbacks.Deallocate += MultiplayEventCallbacks_Deallocate;
                multiplayEventCallbacks.Error += MultiplayEventCallbacks_Error;
                multiplayEventCallbacks.SubscriptionStateChanged += MultiplayEventCallbacks_SubscriptionStateChanged;

                _serverQueryHandler =
                    await MultiplayService.Instance.StartServerQueryHandlerAsync(10, "gesture_competitive", "classic",
                        "test_build", "classic");

        }
        private void MultiplayEventCallbacks_Allocate(MultiplayAllocation args)
        {
            Debug.Log("Allocation event received");
            Debug.Log($"Server Id: {args.ServerId}");
            Debug.Log($"Allocation Id: {args.AllocationId}");
            Debug.Log($"Event Id: {args.EventId}");

            var serverConfig = MultiplayService.Instance.ServerConfig;
            Debug.Log($"Server ID[{serverConfig.ServerId}]");
            Debug.Log($"AllocationID[{serverConfig.AllocationId}]");
            Debug.Log($"Port[{serverConfig.Port}]");
            Debug.Log($"QueryPort[{serverConfig.QueryPort}");
            Debug.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");

            string ipv4Address = serverConfig.IpAddress;

            ushort port = serverConfig.Port;

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port, "0.0.0.0");

            NetworkManager.Singleton.StartServer();
            ServerInputSystem.SetActive(true);
            Debug.Log("Server is ready to accept players");
            MultiplayService.Instance.ReadyServerForPlayersAsync();
        }

        private void MultiplayEventCallbacks_Deallocate(MultiplayDeallocation args)
        {
            Debug.Log("Deallocation event received");
            Debug.Log($"Server Id: {args.ServerId}");
            Debug.Log($"Allocation Id: {args.AllocationId}");
            Debug.Log($"Event Id: {args.EventId}");
        }

        //error
        private void MultiplayEventCallbacks_Error(MultiplayError error)
        {
            Debug.LogError("Error event received");
            Debug.LogError($"Error Detail: {error.Detail}");
            Debug.LogError($"Error Reason: {error.Reason}");
        }

        private void MultiplayEventCallbacks_SubscriptionStateChanged(MultiplayServerSubscriptionState state)
        {
            Debug.Log($" Subscription state changed to {state}");
        }
#endif

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }
            else
            {
                _gameController = this;
            }

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFPS;
            SessionManager.ReadCommandArgs(this);
            gameProperties = GetComponent<GameProperties>();


#if DEDICATED_SERVER
            Global.eventManager.onServicesInitilalised += ListenServerEvents;
#endif

#if !DEDICATED_SERVER

            ServerBrowser.ConnectToServer();
#endif


            OnClientConnectedCallback += ClientConnected;

            OnClientDisconnectCallback += ClientDisconnected;
        }

        private void ClientConnected(ulong clientId)
        {


            if (IsServer)
            {
                var client = ConnectedClients[clientId];
                var player = client.PlayerObject.GetComponent<NetworkPlayerProcessor>();
                // глобальная логика плеера и сразу какая-то странная инициализация оружий
                _playersDict.Add(clientId, player);

                _playersDict[clientId].data.onPlayerInitialized.AddListener(()=>
                {
                    // PoolNetworkPrefabs(clientId);
                    // if (ConnectedClients.Count >= gameProperties.playerCount)
                    // {
                    //     OnTestServerRpc(clientId);
                    // }
                });
                onPoolPrefabs.AddListener(() =>
                {
                  //  StartGameSessionServerRpc();
                  
                });

                // l.rl("position: " + client.PlayerObject.transform.position);
            }

            if (!IsServer)
            {
                l.rl(LocalClient.PlayerObject.name + " constructed!");
            }

        }
        public void PoolNetworkPrefabs(ulong clientId)
        {
            Debug.Log("        public void PoolNetworkPrefabs() " + clientId);
            // spawn abilities
            _playersDict[clientId].data.abilityController.SpawnWeapons(_playersDict[clientId].data.characterController.characterConfigs, _playersDict[clientId].transform);
            
            Dictionary<string, ulong[]> dict = new();
            foreach (var key in _playersDict[clientId].data.abilityController.abilitiesLib.characterAbilities.Keys)
            {
                var names = _playersDict[clientId].data.abilityController.abilitiesLib.characterAbilities[key].Keys.ToArray();
                ulong[] ids= new ulong[names.Length];
            
                for(int i = 0; i < names.Length; i ++)
                {
                    if(_playersDict[clientId].data.abilityController.abilitiesLib.characterAbilities[key][names[i]].TryGetNetcodeId(out ulong id))
                        ids[i] = id;
                }
                dict.Add(key, ids);
            }
            
            // say client to spawn characters and abilities
            var j = JsonConvert.SerializeObject(dict);
            Debug.Log("Call client rpc in " + clientId + j);
            PoolNetworkPrefabsClientRpc(clientId, j);
            
            onPoolPrefabs?.Invoke();
        }
        
    
        [ClientRpc]
        public void PoolNetworkPrefabsClientRpc(ulong clientId, string weapons)
        {
           // Debug.Log(" [ClientRpc] public void PoolNetworkPrefabsClientRpc(string weapons) " + name);

            if (!IsServer)
            {
                _playersDict[clientId].data.abilityController
                    .SetSpawnedWeapons(JsonConvert.DeserializeObject<Dictionary<string, ulong[]>>(weapons));
                onPoolPrefabs?.Invoke();
            }
        }
        [ServerRpc]
        private void StartGameSessionServerRpc()
        {
            foreach (var player in _playersDict.Keys)
            {
                StartGameSessionClientRpc(player);
            }
        }

        [ClientRpc]
        private void StartGameSessionClientRpc(ulong playerId)
        {
      
            if (!_playersDict[playerId].IsOwner)
            {
                return;
            }
            //    Debug.Log(" [Client rpc] private void StartGameSessionClientRpc(ulong playerId) "  + playerId);
            _playersDict[playerId].data.abilityController
                .AddCharacterToInventory(_playersDict[playerId].data.characterController.currentCharacter.name);

            if (gameProperties != null && gameProperties.debugCharacterAbilities != null)
            {
                foreach (var VARIABLE in gameProperties.debugCharacterAbilities)
                {
                    _playersDict[playerId].data.abilityController.AddCharacterToInventory(VARIABLE.ToString());
                }
            }

            _playersDict[playerId].data.abilityController.UseCharacterAbilities();
        }
        private void ClientDisconnected(ulong clientId)
        {
            if (_playersDict.ContainsKey(clientId))
                _playersDict.Remove(clientId);
            l.rl(clientId + " disconnected!");
        }
        private void Update()
        {
#if DEDICATED_SERVER
            if (_serverQueryHandler != null)
            {
                if (IsServer)
                {
                    _serverQueryHandler.CurrentPlayers = (ushort)NetworkManager.Singleton.ConnectedClients.Count;
                }

                _serverQueryHandler.UpdateServerCheck();
            }
#endif
        }
    }
}