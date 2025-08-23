using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts
{
    public class PokeInteractButton : NetworkBehaviour
    {
        [SerializeField] private GameObject _controlledObject;
        private IButtonControllable _controllable;

        private bool _isActive = true;
        public UnityEvent OnReset;
        
        void Start()
        {
            if (!_controlledObject.TryGetComponent(out _controllable))
            {
                Debug.LogError("Controlled Object must have IButtonControllable");
                return;
            }

            _controllable.OnCompleted += Reset;
        }
        
        //////////////////////////////////
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                InteractServerRPC();
            }
        }
        //////////////////////////////////

        private void Reset()
        {
            _isActive = true;
            OnReset?.Invoke();
        }

        [ServerRpc(RequireOwnership = false)]
        private void InteractServerRPC()
        {
            _isActive = false;
            if (_controllable.IsInteractable) _controllable.Interact();
            
        }
        
        [ClientRpc]
        private void InteractClientRpc()
        {
            
        }
        
        void OnValidate()
        {
            if (_controlledObject == null) return;
            if (!_controlledObject.TryGetComponent(out _controllable))
            {
                Debug.LogWarning("Controlled Object must have IButtonControllable");
            }
        }
    }
}
