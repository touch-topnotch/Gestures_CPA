using System;
using Scripts.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Weapons
{
    public abstract class Melee: Weapon
    {
        [Header("Melee components")]
        [SerializeField] private float _bladeMinSpeed;
        [SerializeField] private Blade _blade;

        [ShowInInspector]
        public int capacity
        {
            get => _capacity;
            set
            {
                _capacity = value;
                if (_capacity <= 0)
                {
                    AbilityReleased();
                    _capacity = 0;
                }
            }
        }
     
        
        private int _capacity;
        
        private Vector3 _previousBladePointPosition;

        private bool _bladeTriggered;
        protected override bool HitImpactCondition(out string affected) => _blade.onHitImpact(out affected);
        protected override bool HitCallCondition() => _blade.speed > _bladeMinSpeed;
        protected override void OnHitImpact(string affected)
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