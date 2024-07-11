using System;
using CrossPlatform.Movement;
using CrossPlatform.PlayerLogic;
using CrossPlatform.Tests;
using UnityEngine;
using UnityEngine.Serialization;

namespace CrossPlatform.Scripts
{
    public delegate void UpdateDelegate();

    public class Initializer : MonoBehaviour
    {
        [SerializeField]
        private GameObject playerPrefab;

        [SerializeField] private GestureFramesRecorder recorder;
        
        private Player _player;
        private UpdateDelegate _updateDelegate;
        
        public void Awake()
        {
            SpawnPlayers();
            recorder.Initialize(_player);
        }

        public void SpawnPlayers()
        {
            var prefab = GameObject.Instantiate(this.playerPrefab);
            _player = prefab.GetComponent<Player>();
            _player.Initialize();
        }
        private void FixedUpdate()
        {
            _updateDelegate?.Invoke();
        }
    }
}