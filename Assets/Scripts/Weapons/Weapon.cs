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
        HitHolds,
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


    /// <summary>
    /// This class describes all SERVER logic of the weapon. Don't try to use it as front-part of weapon, only hit logic,
    /// which involves other players.
    /// For managing gesture side of this logic (for example, hit condition is (player gesture == palm)) - manipulate
    /// of all calculations in the client side inside inherited script and call serverRPC method for change condition
    /// when you want to make a hit.
    /// More simple:
    /// <example>
    /// private onHit;
    /// hitConditionServerRPC(bool onHit) => this.onHit = onHit;
    /// hitCondition = onHit;
    /// update()
    ///     if(isClient && isOwner && gesture == palm)
    ///         hitConditionServerRPC(true);
    /// </example>
    /// </summary>
    public abstract class Weapon : NetworkRecognizableComponent, IGrabable
    {
        [Header("Weapons components")] [SerializeField]
        protected WeaponDesign weaponDesign;

        [field: SerializeField] public GrabSystem GrabSystem { get; set; }

        protected int _power;


        private bool _canHitCall;
        
        private State _state;
        protected  State state
        {
            get => _state;
            set
            {
                _state = value;
                if (value != State.HitCalled)
                {
                    _canHitCall = true;
                }
            }
        }
        protected UpdateEvent _onUpdate => UpdateEvent.Instance;

 
        /// <summary>
        /// <remarks>
        /// Server-Only! Don't use a lot of memory and time inside, иначе придется продать почку на хостинг сервера
        ///</remarks>
        /// This function is called when the player grabs the weapon and it able to shoot
        /// </summary>
        protected virtual void OnWeaponGrabbed(){}
        
        /// <summary>
        /// <remarks>
        /// Server-Only! Don't use a lot of memory and time inside, иначе придется продать почку на хостинг сервера
        ///</remarks>
        /// The hit call ability.
        /// <example>
        /// katana speed is higher than minimum speed
        /// </example>
        /// </summary>
        protected abstract bool HitCondition();
        
        /// <summary>
        /// <remarks>
        /// Server-Only! Don't use a lot of memory and time inside, иначе придется продать почку на хостинг сервера
        ///</remarks>
        /// <example>
        /// bullet inside the target, player in hammer-hit area
        /// </example>
        /// This function describes the hit impact ability.
        /// </summary>
        /// <param name="affected"> affected object tag (Player/Floor/Map/Others..)</param>
        /// <returns></returns>
        protected abstract bool ImpactCondition(out string affected);

        /// <summary>
        /// This function is called when player tried to hit
        /// <example>
        /// Player moves katana, press the pistol button, make some gesture
        /// </example>
        /// </summary>
        protected virtual void OnHit(){}

        /// <summary>
        /// This function is called when weapon hit condition is met. For example, when bullet hit the target.
        /// </summary>
        /// <param name="affected"> affected object tag (Player/Floor/Map/Others..)</param>
        protected virtual void OnImpact(string affected) {}

        public void Start()
        {
            SetGrabSystemPlayerData();
        }

        public void SetGrabSystemPlayerData()
        {
            GrabSystem.OnGrabStart += OnGrabbed;
            GrabSystem.OnGrabEnd += OnUnGrabbed;
        }

        public virtual void OnGrabbed()
        {
            weaponDesign.OnGrabbed();
        }

        public virtual void OnUnGrabbed()
        {
            weaponDesign.OnUnGrabbed();
        }


        [ClientRpc]
        private void PlayWeaponDesignClientRpc(string props)
        {
            if (!IsClient)
                return;
            
            var keywords = props.Split();
         
            switch (keywords[0])
            {
                case "L":
                    weaponDesign.OnHitHolds();
                    return;
                case "H":
                    weaponDesign.OnHit();
                    return;
                case "I":
                    if (keywords.Length < 2)
                        return;
                    weaponDesign.OnImpact(keywords[1]);
                    return;
                case "A":
                    weaponDesign.OnAbilityReleased();
                    return;
                
            }
        }
        [ServerRpc]
        protected void StartShootingServerRPC()
        {
            if (!IsServer)
                return;
            OnWeaponGrabbed();
            PlayWeaponDesignClientRpc("L");            
            state = State.HitHolds;
            _onUpdate.AddListener(AbilityShootingProcess);
        }

        private void AbilityShootingProcess()
        {
            switch (state)
            {
                case State.HitHolds: // ожидаем выстрела
                    HandleHitCall();
                    return;
                case State.HitCalled: //  нажали на курок
                    if (!_canHitCall) 
                        return;
                    PlayWeaponDesignClientRpc("H");
                    _canHitCall = false;
                    HandleHitImpact();
                    return;
                case State.HitImpact: // попали
                    _onUpdate.RemoveListener(AbilityShootingProcess);
                    return;
            }
        }

        private void HandleHitCall()
        {
            if (HitCondition())
            {
                if (HitCondition() && state == State.HitHolds)
                {
                    state = State.HitCalled;
                }
                else
                {
                    state = State.HitHolds;
                }
            }
        }

        private void HandleHitImpact()
        {
            if ((IsServer) && ImpactCondition(out string affected))
            {
                state = State.HitImpact;
                OnImpact(affected);
                PlayWeaponDesignClientRpc("I " + affected);
            }
            else if (!HitCondition())
            {
                StartShootingServerRPC();
            }
        }

        public sealed override void OnFrameRecognized(string name)
        {
            if (IsClient)
                weaponDesign.OnFrameRecognized(name);
        }

        public sealed override void AbilityCalled()
        {
            if (IsClient)
                weaponDesign.OnGestureDetected();
        }

        protected sealed override void OnAbilityReleased()
        {
            _onUpdate.RemoveListener(AbilityShootingProcess);
            if (IsClient)
                weaponDesign.OnAbilityReleased();
        }
        // write implementation here
        protected override bool shouldAddMissingComponents { get; }
        //
        
        // write implementation here
        public override void AddMissingComponents()
        {
        }
    }
}