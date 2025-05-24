using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Weapons;
using UnityEngine;

namespace Scripts
{
    public class Lightning : Magic
    {
        [Header("Lightning components")] 
        [SerializeField] private Blade triggerZone;

        private void Start()
        {
            StartShootingServerRPC();
        }

        protected override bool ImpactCondition(out string affected) => triggerZone.onHitImpact(out affected);

        protected override bool HitCondition() => true;

        protected override void OnImpact(string affected)
        {
            switch (affected)
            {
                case "Player":
                    Debug.Log("Melee weapon hit player!");
                    mana -= 10;
                    break;
                case "Map":
                    Debug.Log("Melee weapon hit solid object");
                    mana -= 5;
                    break;
                default:
                    Debug.Log("Melee weapon hit something");
                    mana -= 5;
                    break;
            }
        }
    }
}
