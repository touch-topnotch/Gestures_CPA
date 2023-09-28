using System.Collections.Generic;
using Scripts.Network.NetworkManagers;
using Scripts.PlayerLogic;
using Scripts.Static;
using Unity.Netcode;
using UnityEngine;

namespace DedicatedServers.LobbyServer
{
    public class LobbyServerManager: CustomNetworkBehaviour
    {
        public List<NetworkUser> users = new();
        public Transform[] spawnPoints;
        private readonly UsersSpawner _usersSpawner = new();

        protected override void Awake()
        {
            base.Awake();
            Network.OnServerStarted += () => { Debug.Log("Lobby server started!"); };
           
            Network.OnClientConnectedCallback += ClientConnected;
            Network.OnClientDisconnectCallback += ClientDisconnected;
            Network.StartServer();
        }

        private void ClientConnected(ulong clientId)
        {
            var client = Network.ConnectedClients[clientId];
            l.rl(client.PlayerObject.name + " connected!");
            _usersSpawner.SpawnPlayer(client.PlayerObject, spawnPoints);
            AddUserToList(Network.ConnectedClients[clientId].PlayerObject);
        }
        private void ClientDisconnected(ulong clientId)
        {
            var client = Network.ConnectedClients[clientId];
            l.rl(client.PlayerObject.name + " disconnected!");
            RemoveUserFromList(client.PlayerObject.GetComponent<NetworkUser>());
        }
        
        private void AddUserToList(NetworkObject user)
        {
            NetworkUser netUser = user.GetComponent<NetworkUser>();
            users.Add(netUser);
        }

        private void RemoveUserFromList(NetworkUser user)
        {
            users.Remove(user);
        }
     
    }
}