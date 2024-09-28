using Scripts.Network;
using Unity.Netcode;
using UnityEngine;

namespace DedicatedServers.LobbyServer
{
    public class UsersSpawner
    {
        
        private int _spawnId = 0;
        public void SpawnPlayer(NetworkObject networkPlayer, in Transform[] spawnPoints)
        {
            Vector3 newPosition = GetNextSpawnPoint(spawnPoints);
            networkPlayer.transform.position = newPosition;
        }
        private Vector3 GetNextSpawnPoint(in Transform[] spawnPoints)
        {
            // Implement your logic to select a spawn point from the spawnPoints list
            // For example, you can use a simple round-robin approach:
        
            Transform spawnPoint = spawnPoints[_spawnId];
            _spawnId = (_spawnId + 1) % spawnPoints.Length;
            return spawnPoint.position;
        }
       
    }
}