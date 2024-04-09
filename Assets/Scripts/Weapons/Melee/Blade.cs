using Gesture_Editor_SDK.ReadOnly;
using UnityEngine;

namespace Scripts.Weapons
{
    [RequireComponent(typeof(Rigidbody))]
    public class Blade : MonoBehaviour, Hittable
    {
        [SerializeField] private Transform bladePoint;

        [ReadOnlyInInspector]
        [SerializeField] private Vector3 _speed;

        private Vector3 _prevPosition;

        public float speed => _speed.magnitude;
        public Vector3 speedVec => _speed;
        private bool lastTrigger = false;
        private string lastName = "";
        
        public void OnTriggerEnter(Collider other)
        {
            lastTrigger = true;
            lastName = other.tag;
            Debug.Log("Trigger Enter");
        } 
        
        public void OnTriggerExit(Collider other)
        {
            lastTrigger = false;
            lastName = "";
            Debug.Log("Trigger Exit");
        }
        private void Update()
        {
            var position = bladePoint.position;
            _speed = (position - _prevPosition) / Time.deltaTime;
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