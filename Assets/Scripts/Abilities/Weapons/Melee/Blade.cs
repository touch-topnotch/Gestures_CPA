using Gesture_Editor_SDK.ReadOnly;
using Scripts.Components;
using Scripts.Static.Definitions;
using UnityEngine;

namespace Scripts.Weapons
{
    [RequireComponent(typeof(Rigidbody))]
    public class Blade : TriggerBehaviour //, Hittable
    {
        [SerializeField] private Transform bladePoint;

        [ReadOnlyInInspector] [SerializeField] private Vector3 _speed;

        private Vector3 _prevPosition;

        public float speed => _speed.magnitude;
        public Vector3 speedVec => _speed;
        private bool lastTrigger = false;
        private bool isTrigging = false;
        private Affected lastAffected;

        private void Awake()
        {
            TriggerEnterEvent.AddListener(OnAffectedEnter);
            TriggerExitEvent.AddListener(OnAffectedExit);
        }

        public void OnAffectedEnter(Affected other)
        {
            lastTrigger = true;
            isTrigging = true;
            lastAffected = other;
            Debug.Log("Trigger Enter");
        }

        public void OnAffectedExit(Affected other)
        {
            isTrigging = false;
            Debug.Log("Trigger Exit");
        }

        private void Update()
        {
            var position = bladePoint.position;
            _speed = (position - _prevPosition) / Time.deltaTime;
            _prevPosition = position;
        }

        public bool onHitImpact()
        {
            if (lastTrigger && isTrigging)
            {
                lastTrigger = false;
                isTrigging = false;
                return true;
            }

            return false;
        }
    }
}