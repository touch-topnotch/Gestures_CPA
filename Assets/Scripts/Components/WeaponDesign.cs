using Scripts.PlayerLogic;
using Scripts.Systems;
using UnityEngine;
using UnityEngine.Events;

namespace Components
{
    /// <summary>
    /// The methods are called in the order described below.
    /// <param name="OnFrameRecognized">First, frame recognition </param>
    /// <param name="OnGestureCasted">invokes after all frames. </param> 
    /// <param name= "OnActivated">Next, we wait for the player to activate the weapon (maybe take it, or it will happen immediately after the cast),</param> 
    /// <param name= "OnHitStarted">after which the weapon is used and the hit (взмах меча, нажатие на курок..)</param>
    /// <param name= "OnImpact"> and impact (попадание) are described. </param>
    /// <param name= "OnHitStopped">after hit might be stopped (speed of sword less than 5, cull down, no mana</param> 
    /// <param name= "OnDeactivated">Further, the weapon can simply be deactivated</param>
    /// <param name= "OnAbilityReleased">or released (needs to cast it again)</param>
    /// </summary>
    public abstract class WeaponDesign : PrefabSerializedMonoBehaviour
    {
        [Header("Components")]
        public AudioProcessor audioProcessor;
        public VFXProcessor vfxProcessor;
        [HideInInspector] public PlayerData playerData;
        [HideInInspector]
        public UnityAction[] actions;
        [HideInInspector]
        public UnityAction<string>[] paramActions;
        public abstract void OnReadyToBeCasted();
        public abstract void OnCastCancelled();
        public abstract void OnGestureCasted();
        public abstract void OnActivated();
        public abstract void OnHitStarted();
        public abstract void OnHitStopped();
        public abstract void OnDeactivated();
        public abstract void OnAbilityDestroyed();
        public abstract void OnFrameRecognized(string frameName);
        public abstract void OnImpact(string affected);

        private void Awake()
        {
            SetActions();
        }
        public void SetActions()
        {
            actions = new UnityAction[]
            {
                OnReadyToBeCasted,
                OnCastCancelled,
                OnGestureCasted,
                OnActivated,
                OnDeactivated,
                OnHitStarted,
                OnHitStopped,
                OnAbilityDestroyed
            };
            paramActions = new UnityAction<string>[]
            {
                OnFrameRecognized,
                OnImpact
            };
        }

        protected override bool shouldAddMissingComponents => !(vfxProcessor && audioProcessor);
    }
}