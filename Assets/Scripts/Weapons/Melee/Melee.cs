using UnityEngine;

namespace Scripts.Weapons
{
    public class Melee : Weapon, IGrabable
    {
        [field: SerializeField] public GrabSystem GrabSystem { get; set; }
        
        [Header("Melee components")] [SerializeField]
        protected float _bladeMinSpeed;

        [SerializeField] protected Blade _blade;
        
        [SerializeField] private Rigidbody _rigidbody;

        

        private Vector3 _previousBladePointPosition;

        private bool _bladeTriggered;
        
        public void Start()
        {
            SetGrabSystem();
        }

        public void SetGrabSystem()
        {
            GrabSystem.OnGrabStart += OnGrabbed;
            GrabSystem.OnGrabEnd += OnUnGrabbed;
        }

        public virtual void OnGrabbed()
        {
            weaponDesign.OnGrabbed();
            _rigidbody.isKinematic = true;
            StartShootingServerRPC();
        }
        
        public virtual void OnUnGrabbed()
        {
            weaponDesign.OnUnGrabbed();
            _rigidbody.isKinematic = false;
        }

        protected override bool ImpactCondition(out string affected) => _blade.onHitImpact(out affected);

        protected override bool HitCondition() => _blade.speed > _bladeMinSpeed;

        protected override void OnImpact(string affected)
        {
            switch (affected)
            {
                case "Player":
                    Debug.Log("Melee weapon hit player!");
                    Power -= 10;
                    break;
                case "Map":
                    Debug.Log("Melee weapon hit solid object");
                    Power -= 5;
                    break;
            }
        }
    }
}