using Scripts.PlayerLogic;
using Scripts.Systems;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components
{
    public abstract class WeaponDesign : PrefabSerializedMonoBehaviour
    {
        public AudioProcessor audioProcessor;
        public VFXProcessor vfxProcessor;

        [HideInInspector] public PlayerData playerData;

        public abstract void OnFrameRecognized(string frameName);

        public abstract void OnGestureDetected();
        public abstract void OnHit();
        public abstract void OnImpact(string affected);
        public abstract void OnAbilityReleased();
        public abstract void OnHitHolds();

        public virtual void OnGrabbed()
        {
        }
        
        public virtual void OnUnGrabbed()
        {
        }
    }
}