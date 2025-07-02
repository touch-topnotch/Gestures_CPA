using System;
using Scripts.Static.Definitions;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Components
{
    [RequireComponent(typeof(Rigidbody))]
    public class TriggerBehaviour : SmartComponent
    {
        public Rigidbody rigidBody { get; protected set; }
        public UnityEvent<Affected> TriggerEnterEvent;
        public UnityEvent<Affected> TriggerExitEvent;
        public virtual bool ImpactCondition() => true;

        public void OnTriggerEnter(Collider other)
        {
            if (!ImpactCondition())
                return;

            var ph = new Affected(other);
            if (ph.physicLayer != PhysicLayer.NONE && ph.surfaceType != SurfaceType.NONE)
                TriggerEnterEvent?.Invoke(ph);
        }

        public void OnTriggerExit(Collider other)
        {
            if (!ImpactCondition())
                return;

            var ph = new Affected(other);
            if (ph.physicLayer != PhysicLayer.NONE && ph.surfaceType != SurfaceType.NONE)
                TriggerExitEvent?.Invoke(ph);
        }

        protected override bool shouldAddMissingComponents => !rigidBody;

        public override void AddMissingComponents()
        {
            rigidBody ??= GetComponent<Rigidbody>();
        }
    }
}