using System.Collections.Generic;
using Scripts.Network;
using Scripts.PlayerLogic;
using Scripts.Static;
using Unity.Netcode;
using UnityEngine;

namespace Scripts.GameControllers
{
    public class GameController: NetworkBehaviour
    {
        private static GameController _gameController;
        public static GameController Instance => _gameController;

        private Dictionary<ulong, NetworkPlayerProcessor> _playersDict = new Dictionary<ulong, NetworkPlayerProcessor>();
        
        [SerializeField] private GameObject ServerInputSystem;
        
        [SerializeField] private NetworkManager _networkManager;
        public Dictionary<ulong, NetworkPlayerProcessor> PlayersDict => _playersDict;
        
        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }
            else
            {
                _gameController = this;
            }
            
            SessionManager.Connect(_networkManager);
            
            _networkManager.OnClientConnectedCallback += ClientConnected;
            _networkManager.OnClientDisconnectCallback += ClientDisconnected;
            _networkManager.OnServerStarted += ()=>
            {
                ServerInputSystem.SetActive(IsServer);
            };
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
            if(_playersDict.ContainsKey(clientId))
                _playersDict.Remove(clientId);
            l.rl(clientId + " disconnected!");
        }
    }
}