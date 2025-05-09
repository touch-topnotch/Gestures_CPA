using System.Collections.Generic;
using Scripts.Events;
using Scripts.Network;
using Scripts.PlayerLogic;
using Scripts.Static;
using Unity.Netcode;
using UnityEngine;

#if DEDICATED_SERVER
using Unity.Netcode.Transports.UTP;
using Unity.Services.Multiplay;
#endif

namespace Scripts.GameControllers
{
    public class GameController : NetworkBehaviour
    {
        public const int targetFPS = 60;

        private static GameController _gameController;
        public static GameController Instance => _gameController;

        private readonly Dictionary<ulong, NetworkPlayerProcessor>
            _playersDict = new Dictionary<ulong, NetworkPlayerProcessor>();

        [SerializeField] private GameObject ServerInputSystem;

        [SerializeField] private NetworkManager _networkManager;
        public Dictionary<ulong, NetworkPlayerProcessor> PlayersDict => _playersDict;

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

            SessionManager.ReadCommandArgs(_networkManager);
#if DEDICATED_SERVER
            EventInitializer.Instance.onServicesInitilalised += ListenServerEvents;
#endif
#if !DEDICATED_SERVER

            ServerBrowser.ConnectToServer();
#endif


            _networkManager.OnClientConnectedCallback += ClientConnected;
            _networkManager.OnClientDisconnectCallback += ClientDisconnected;
        }

        private void ClientConnected(ulong clientId)
        {
            if (!_networkManager.IsServer)
            {
                l.rl(_networkManager.LocalClient.PlayerObject.name + " constructed!");
            }

            if (_networkManager.IsServer)
            {
                var client = _networkManager.ConnectedClients[clientId];
                var player = client.PlayerObject.GetComponent<NetworkPlayerProcessor>();

                _playersDict.Add(clientId, player);
                l.rl("position: " + client.PlayerObject.transform.position);
            }
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