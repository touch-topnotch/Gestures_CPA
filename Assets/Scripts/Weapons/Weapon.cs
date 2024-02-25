using System;
using Components;
using Gesture_Editor_SDK.Realtime;
using Scripts.Events;
using Scripts.PlayerLogic;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Weapons
{
    public enum State
    {
        HitHolding,
        HitCalled,
        HitImpact,
    }
    public abstract class Weapon : NetworkRecognizableBehaviour
    {
       [Header("Weapons components")]
        [SerializeField]
        protected AudioProcessor audioProcessor;
        
        protected readonly NetworkVariable<State> state = new NetworkVariable<State>();
        protected UpdateEvent _onUpdate => UpdateEvent.Instance;
        protected abstract bool HitImpactCondition(out string affected);
        protected abstract bool HitCallCondition();
        protected abstract void OnHitStartHold();
        protected abstract void OnHitHolding();
        protected abstract void OnHitCalled();
        protected abstract void OnHitImpact(string affected);

        [ClientRpc]
        private void OnHitImpactClientRpc(string affected)
        {
            if (IsServer)
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
            if (IsOwner && IsClient && HitCallCondition())
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
            if (IsServer && HitImpactCondition(out string affected))
            {
                state.Value = State.HitImpact;
                OnHitImpact(affected);
                OnHitImpactClientRpc(affected);
            }
        }

    }
}
