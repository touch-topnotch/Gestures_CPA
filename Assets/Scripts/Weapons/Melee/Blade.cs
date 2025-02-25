using System;
using Gesture_Editor_SDK.ReadOnly;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Scripts.Weapons
{
    [RequireComponent(typeof(Rigidbody))]
    public class Blade : MonoBehaviour, Hittable
    {
        [SerializeField] private Transform bladePoint;

        [ReadOnlyInInspector]
        [SerializeField] private float _speed;

        private Vector3 _prevPosition;

        public float speed => _speed;
        private bool lastTrigger = false;
        private string lastName = "";
        
        public void OnTriggerEnter(Collider other)
        {
            lastTrigger = true;
            lastName = other.tag;
        }
        private void Update()
        {
            var position = bladePoint.position;
            _speed = (position - _prevPosition).magnitude / Time.deltaTime;
            _prevPosition = position;
        }

        public bool onHitImpact(out string tag)
        {
            tag = lastName;
            
            if (lastTrigger)
            {
                lastTrigger = false;
                return true;
            }
            
            return false;
        }
    }
}