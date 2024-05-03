using Scripts.PlayerLogic;
using Scripts.Systems;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Components
{
    public abstract class WeaponDesign: PrefabSerializedMonoBehaviour
    {
        public AudioProcessor audioProcessor;
        public VFXProcessor vfxProcessor;

        [HideInInspector] public PlayerData playerData;

        public abstract void OnFrameRecognized(string frameName);
        
        public abstract void OnGestureDetected();
        public abstract void OnHitHolding();
        public abstract void OnHitCalled();
        public abstract void OnHitImpact(string affected);
        public abstract void OnAbilityReleased();
    }
}