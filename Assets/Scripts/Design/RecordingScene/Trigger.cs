using System;
using Scripts.Events;
using UnityEngine;
using Zenject;

namespace Design
{
    [RequireComponent(typeof(Rigidbody))]
    public class Trigger: MonoBehaviour
    {
        public Action<string> OnEnter;
        public Action<string> OnExit;

        private void OnTriggerEnter(Collider other)
        {
            OnEnter?.Invoke(other.tag);
        }

        private void OnTriggerExit(Collider other)
        {
            OnExit?.Invoke(other.tag);
        }
    }
}