using System;
using CrossPlatform.Movement;
using CrossPlatform.PlayerLogic;
using CrossPlatform.Tests;
using UnityEngine;
using UnityEngine.Serialization;

namespace CrossPlatform.Scripts
{
    public delegate void OnUpdate();

    public class Initializer : MonoBehaviour
    {
        [SerializeField]
        private GameObject playerPrefab;

        [SerializeField] private GestureFramesRecorder recorder;
        
        private Player _player;
        private OnUpdate _onUpdate;
        
        public void Awake()
        {
            SpawnPlayers();
            recorder.Initialize(_player);
        }

        public void SpawnPlayers()
        {
            var prefab = GameObject.Instantiate(this.playerPrefab);
            if (!prefab.GetComponent<Player>())
            {
                Debug.LogWarning($"{playerPrefab.name} prefab hasn't Player component!");
                return;
            }

            _player = prefab.GetComponent<Player>();
            _player.Initialize();
        }
        private void FixedUpdate()
        {
            _onUpdate?.Invoke();
        }
    }
}