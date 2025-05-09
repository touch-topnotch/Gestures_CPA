using Unity.Netcode;
using UnityEngine;

namespace Scripts.GameControllers
{
    public class UsersSpawner
    {
        private int _spawnId = 0;

        public void SpawnPlayer(NetworkObject networkPlayer, in Transform[] spawnPoints)
        {
            Vector3 newPosition = GetNextSpawnPoint(spawnPoints);
            networkPlayer.transform.position = newPosition;
        }

        public void SpawnPlayer(Transform anchor, in Transform[] spawnPoints)
        {
            Vector3 newPosition = GetNextSpawnPoint(spawnPoints);
            anchor.position = newPosition;
        }

        private Vector3 GetNextSpawnPoint(in Transform[] spawnPoints)
        {
            // Implement your logic to select a spawn point from the spawnPoints list
            // For example, you can use a simple round-robin approach:
            Debug.Log("SpawnId: " + _spawnId);
            Transform spawnPoint = spawnPoints[_spawnId];
            _spawnId = (_spawnId + 1) % spawnPoints.Length;
            return spawnPoint.position;
        }

        public static Vector3 GetLocalSpawnPoint(in ulong id, in Transform[] spawnPoints)
        {
            return spawnPoints[(int)id % spawnPoints.Length].position;
        }
    }
}