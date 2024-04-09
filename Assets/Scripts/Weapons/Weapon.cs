using System;
using Components;
using Gesture_Editor_SDK.Realtime;
using Scripts.Events;
using Scripts.PlayerLogic;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Scripts.Weapons
{
    public enum State
    {
        HitHolding,
        HitCalled,
        HitImpact,
    }

    public enum WeaponClass
    {
        Melee,
        Magic,
        Range,
        Custom
    }
    public abstract class Weapon : NetworkRecognizableBehaviour
    {
        [Header("Weapons components")] 
        
        [SerializeField]
        protected WeaponDesign weaponDesign;
        
        protected readonly NetworkVariable<State> state = new NetworkVariable<State>();
        protected UpdateEvent _onUpdate => UpdateEvent.Instance;

        [SerializeField] private bool _offlineTest; 
        protected abstract bool HitImpactCondition(out string affected);
        protected abstract bool HitCallCondition();

        protected virtual void OnHitStartHold()
        {
        }

        protected virtual void OnHitHolding()
        {
            if (IsClient || _offlineTest)
                weaponDesign.OnHitHolding();
        }

        protected virtual void OnHitCalled()
        {
            if (IsClient || _offlineTest)
                weaponDesign.OnHitCalled();
        }

        protected virtual void OnHitImpact(string affected)
        {
            if (IsClient || _offlineTest)
                weaponDesign.OnHitImpact(affected);
        }

        public override void OnNetworkSpawn()
        {
            if (IsClient || _offlineTest)
                weaponDesign.SetPlayerData(playerData);
        }

        [ClientRpc]
        private void OnHitImpactClientRpc(string affected)
        {
            if (IsServer || _offlineTest)
                return;
            
            OnHitImpact(affected);
        }
  
        protected void StartShooting()
        {
            state.Value = State.HitHolding;
            OnHitStartHold();
            _onUpdate.AddListener(AbilityShootingProcess);
        }
        private void AbilityShootingProcess()
        {
            switch (state.Value)
            {
                case State.HitHolding: // ожидаем выстрел
                    OnHitHolding();
                    HandleHitCall();
                    return;
                case State.HitCalled: //  нажали на курок
                    OnHitCalled();
                    HandleHitImpact();
                    return;
                case State.HitImpact: // попали
                    _onUpdate.RemoveListener(AbilityShootingProcess);
                    return;
            }
        }
        private void HandleHitCall() 
        {
            if ((IsOwner && IsClient  || _offlineTest) && HitCallCondition())
            {
                if (HitCallCondition() && state.Value == State.HitHolding)
                {
                    state.Value = State.HitCalled;
                }
                else
                {
                    state.Value = State.HitHolding;
                }
                   
            }
        }

        private void HandleHitImpact()
        {
            if ((IsServer || _offlineTest) && HitImpactCondition(out string affected))
            {
                state.Value = State.HitImpact;
                OnHitImpact(affected);
                OnHitImpactClientRpc(affected);
            }
            else if (!HitCallCondition())
            {
                state.Value = State.HitHolding;
            }
        }

        public override void OnFrameRecognized(string name)
        {
            if(IsClient || _offlineTest)
                weaponDesign.OnFrameRecognized(name);
        }

        public override void AbilityCalled()
        {
          if(IsClient || _offlineTest)
              weaponDesign.OnGestureDetected();
        }

        protected override void OnAbilityReleased()
        {
            if(IsClient || _offlineTest)
               weaponDesign.OnAbilityReleased();
        }
    }
}
