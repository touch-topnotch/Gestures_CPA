using UnityEngine;

namespace Scripts.Gestures.Classes
{
    public class Melee: GestureCall
    {
        [Range(0, 100f)] protected float endurance;
        protected virtual void Hit(float damage)
        {
            
        }

    }
}