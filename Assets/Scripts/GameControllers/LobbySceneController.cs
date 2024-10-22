using System.Collections.Generic;
using DedicatedServers.LobbyServer;
using Scripts.Network;
using Scripts.PlayerLogic;
using Scripts.Static;
using Unity.Netcode;
using UnityEngine;

namespace Scripts.GameControllers
{
    public class LobbySceneController : NetworkBehaviour
    {
        private List<Player> _players = new List<Player>();
        public NetworkManager _networkManager;
        public Transform[] spawnPoints;
        private UsersSpawner _usersSpawner = new();
        public void Awake()
        {
            SessionManager.Connect(_networkManager);
            
            _networkManager.OnClientConnectedCallback += ClientConnected;
            _networkManager.OnClientDisconnectCallback += ClientDisconnected;
        }
        
        private void ClientConnected(ulong clientId)
        {
            if (_networkManager.IsServer)
            {
                var client = _networkManager.ConnectedClients[clientId];
                var player = client.PlayerObject.GetComponent<Player>();
               //Random position by x and z
                client.PlayerObject.transform.position = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
                // _usersSpawner.SpawnPlayer(client.PlayerObject, spawnPoints);
                _players.Add(player);
                l.rl("position: " + client.PlayerObject.transform.position);
            }
        }
        
        private void ClientDisconnected(ulong clientId)
        {
            if (_networkManager.IsServer)
            {
                var client = _networkManager.ConnectedClients[clientId];

                _players.Remove(client.PlayerObject.GetComponent<Player>());
                l.rl(client.PlayerObject.name + " disconnected!");
            }
        }
    }
}