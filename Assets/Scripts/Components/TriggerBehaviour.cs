using System;
using Scripts.Static.Definitions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Components
{
    public class TriggerBehaviour : SmartComponent
    {

        public Rigidbody rigidBody;
        public Collider orbCollider; 
        public UnityEvent<Affected> TriggerEnterEvent;
        public UnityEvent<Affected> TriggerExitEvent;
        public virtual bool ImpactCondition() => true;

        public void EnableComponents()
        {
            orbCollider.enabled = true;
        }
        public void DisableComponents()
        {
            orbCollider.enabled = false;
        }

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

        protected override bool shouldAddMissingComponents => !(rigidBody && orbCollider);

        public override void AddMissingComponents()
        {
            if (!rigidBody)
            {
                if (GetComponent<Rigidbody>())
                    rigidBody = GetComponent<Rigidbody>();
                else
                {
                    var r = transform.AddComponent<Rigidbody>();
                    r.isKinematic = true;
                    r.useGravity = false;
                    rigidBody = r;
                }
            }
            
            if (!orbCollider)
            {
                if (GetComponent<Collider>())
                    orbCollider = GetComponent<Collider>();
                else
                {
                    var sc = transform.AddComponent<SphereCollider>();
                    sc.isTrigger = true;
                    sc.radius = 0.2f;
                    orbCollider = sc;
                }
            }
        }
    }
}