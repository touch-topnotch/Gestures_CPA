using System.Collections;
using System.Collections.Generic;
using Scripts.Weapons;
using UnityEngine;

namespace Scripts
{
    public class Magic : Weapon
    {
        public int mana
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
        protected override bool CanHitCall => true; //

        protected override bool HitCondition() => true;

        protected override bool ImpactCondition(out string affected)
        {
            affected = "";
            return true;
        }
    }
}
