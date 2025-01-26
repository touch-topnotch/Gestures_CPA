using System;
using Scripts.Network;
using UnityEngine;

namespace Gesture_Editor_SDK.Realtime
{
    public abstract class Sword : RecognizableObject
    {
        [Range(0,1)]
        private float _endurance = 1;
        private readonly float _damage = 0.1f;
        public void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.OnDamageTaken(_damage);
            }
            Hit();
        }

        internal void Hit() // called if collision of sword and object detected
        {
            _endurance -= 0.1f;
            if (_endurance > 0)
            {
                OnHit();
            }
            else
            {
                AbilityReleased();
            }
        }
        protected abstract void OnHit();
        public override void AbilityCalled()
        {
            // start hitting
        }

        protected override void OnAbilityReleased()
        {
            // shader dematerialization effect here.
        }
    }
}
