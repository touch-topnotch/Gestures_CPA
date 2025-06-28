using System.Collections;
using System.Collections.Generic;
using Scripts.Weapons;
using UnityEngine;

namespace Scripts
{
    public class Magic : Weapon
    {
        protected override bool CanHitCall => true; //

        protected override bool HitCondition() => true;

        protected override bool ImpactCondition(out string affected)
        {
            affected = "";
            return true;
        }
        
    }
}
