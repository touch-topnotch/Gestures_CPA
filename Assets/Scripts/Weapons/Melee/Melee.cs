using UnityEngine;

namespace Scripts.Weapons
{
    public class Melee : Weapon
    {
        [Header("Melee components")] [SerializeField]
        protected float _bladeMinSpeed;

        [SerializeField] protected Blade _blade;
        
        [SerializeField] private Rigidbody _rigidbody;

        public int capacity
        {
            get => _power;
            protected set
            {
                _power = value;
                if (_power <= 0)
                {
                    AbilityReleased();
                    _power = 0;
                }
            }
        }

        private Vector3 _previousBladePointPosition;

        private bool _bladeTriggered;
        

        public override void OnGrabbed()
        {
            base.OnGrabbed();
            _rigidbody.isKinematic = true;
        }
        
        public override void OnUnGrabbed()
        {
            base.OnGrabbed();
            _rigidbody.isKinematic = false;
            StartShootingServerRPC();
        }

        protected override bool ImpactCondition(out string affected) => _blade.onHitImpact(out affected);

        protected override bool HitCondition() => _blade.speed > _bladeMinSpeed;

        protected override void OnImpact(string affected)
        {
            switch (affected)
            {
                case "Player":
                    Debug.Log("Melee weapon hit player!");
                    capacity -= 10;
                    break;
                case "Map":
                    Debug.Log("Melee weapon hit solid object");
                    capacity -= 5;
                    break;
            }
        }
    }
}