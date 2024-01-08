using UnityEngine;

namespace Scripts.Gestures.Classes
{
    public class Melee: DynamicGesture
    {
        [Range(0, 100f)] protected float endurance;
        
        public Melee(string name) : base(name)
        {
            
        }

        protected virtual void Hit(float damage)
        {
            
        }

    }
}