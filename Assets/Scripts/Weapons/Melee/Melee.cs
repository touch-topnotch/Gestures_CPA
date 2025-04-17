using System;
using Scripts.Events;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Weapons
{
    public class Melee: Weapon
    {
        
        [Header("Melee components")]
        [SerializeField] protected float _bladeMinSpeed;
        [SerializeField] protected Blade _blade;
        
        private Vector3 _previousBladePointPosition;

        private bool _bladeTriggered;

        private void Start()
        {
            StartShooting();
        }

        protected override bool HitImpactCondition(out string affected) => _blade.onHitImpact(out affected);
        protected override bool HitCallCondition() => _blade.speed > _bladeMinSpeed;
        protected override void OnHitImpact(string affected)
        {
            switch (affected)
            {
                case "Player":
                    Debug.Log("Melee weapon hit player!");
                    weaponDesign.OnHitImpact(affected);
                    power -= 10;
                    break;
                case "Map":
                    Debug.Log("Melee weapon hit solid object");
                    weaponDesign.OnHitImpact(affected);
                    power -= 5;
                    break;
            }
            StartShooting();
        }
    }
}