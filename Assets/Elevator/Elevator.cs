using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts
{
    public class Elevator : MonoBehaviour, IButtonControllable
    {
        [SerializeField] private SplineFollow _splineFollower;
        public bool IsInteractable { get; set; } = true;
        public event Action OnCompleted;
        
        
        private bool _reversed;

        private void Start()
        {
            _splineFollower.OnCompleted += Complete;
        }


        public void Interact()
        {
            if (!IsInteractable) return;
            
            IsInteractable = false;
            _splineFollower.StartMovement(_reversed);
            
        }

        private void Complete()
        {
            IsInteractable = true;
            _reversed = !_reversed;
            OnCompleted?.Invoke();
        }
        
        /*void Update()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                _splineFollower.StartMovement();
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                _splineFollower.StartMovement(true);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                _splineFollower.StartMovement(false, true);
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                _splineFollower.StartMovement(true, true);
            }
        }*/

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.transform.SetParent(_splineFollower.transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.transform.SetParent(null);
            }
        }
    }
}
