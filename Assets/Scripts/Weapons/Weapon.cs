using System;
using System.Collections.Generic;
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
        
        [SerializeField]
        [Tooltip("Weapon hit call cooldown")] private float _hitCallDelay = 0.5f;
        private float _hitCallTimer;
        private bool CanHitCall => _hitCallTimer <= 0;
        
        protected readonly NetworkVariable<State> state = new NetworkVariable<State>();
        protected UpdateEvent _onUpdate => UpdateEvent.Instance;
        
        protected abstract bool HitImpactCondition(out string affected);
        protected abstract bool HitCallCondition();

        public void Initialize(PlayerData data)
        {
            Debug.Log(name + " initialized. " + playerData);
                playerData = data;
        }

        protected virtual void OnHitStartHold()
        {
        }

        protected virtual void OnHitHolding()
        {
            if (IsClient)
                weaponDesign.OnHitHolding();
        }

        protected virtual void OnHitCalled()
        {
            if (!CanHitCall) return;

            if (IsClient)
            {
                weaponDesign.OnHitCalled();
                _hitCallTimer = _hitCallDelay;
                _onUpdate.AddListener(UpdateHitCallTimer);
            }
        }
        
        protected virtual void OnHitImpact(string affected)
        {
            if (IsClient)
                weaponDesign.OnHitImpact(affected);
        }

        public override void OnNetworkSpawn()
        {
            // if (IsClient)
            //     weaponDesign.playerData = playerData;
        }

        [ClientRpc]
        private void OnHitImpactClientRpc(string affected)
        {
            if (IsServer)
                return;
            
            OnHitImpact(affected);
        }

        private void UpdateHitCallTimer()
        {
            _hitCallTimer -= Time.deltaTime;
            if (CanHitCall)
                _onUpdate.RemoveListener(UpdateHitCallTimer);
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
            if ((IsOwner && IsClient) && HitCallCondition())
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
            if ((IsServer) && HitImpactCondition(out string affected))
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
            if(IsClient)
                weaponDesign.OnFrameRecognized(name);
        }

        public override void AbilityCalled()
        {
          if(IsClient)
              weaponDesign.OnGestureDetected();
        }

        protected override void OnAbilityReleased()
        {
            if(IsClient)
               weaponDesign.OnAbilityReleased();
        }
    }
}
