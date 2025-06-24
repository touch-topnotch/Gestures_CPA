using System;
using Components;
using Gesture_Editor_SDK.Realtime;
using Scripts.Static.Definitions;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Weapons
{
    /// <remarks>
    /// Важно - теперь это не SERVER-ONLY эвенты, т.е. вы можете вызывать их на любой платформе. Т.е. вы в праве самостоятельно
    /// решать, где и что должно проверяться и запускаться. Например, игрок поднял оружие - смотрим на клиенте, Снимаем врагу хп
    /// - на сервере. Основной плюс в том, что платформу можно поменять за секунду.
    /// Например:
    /// if( IsClient and IsOwner and  grabbs the weapon) then WeaponActivatedEvent.Invoke()
    ///</remarks>
    
    /// <summary>
    /// Я реализовал это просто настолько, насколько возможно. Все описанные ивенты синхронизированы между клиентом и сервером.
    /// Это значит, что вам вообще не нужно думать, какая часть логики должна произойти на платформе, вызовется ли функция +
    /// можно быстро валидировать нагрузку на процессор между игроком и сервером, обрабатывая ивенты в первом или втором
    /// месте
    /// </summary>
    /// <param name="CastStartedEvent">Invokes automatically, when first frame recognized</param>
    /// <param name="CastCancelledEvent">Invokes automatically, or by implementation - when player spent a lot of time for cast</param>
    /// <param name="GestureCastedEvent">Invokes, when dynamic gesture recognized</param>
    /// <param name="ActivatedEvent">Invoke it, when you want to activate it (Grab, Take, Push the button)</param>
    /// <param name="DeactivatedEvent">Invoke it, when you want to deactivate it (Throw, Put, Push the button)</param>
    /// <param name="HitStartedEvent">Invoke it, when the weapon is possible to take damage (katana speed > 0, switch turn on, spell sayed)</param>
    /// <param name="HitStoppedEvent">Invoke it, when the weapons is not possible to take damage (katana speed  == 0, switch turn off) </param>
    /// <param name="AbilityDestroyedEvent">Invoke it, when the weapon should be destroyed</param>
    /// <param name="FrameRecognizedEvent">Invokes automatically, when the frame for cast is recognized</param>
    /// <param name="ImpactEvent">Invoke it, when the weapon hit someone/something</param>
    /// <param name="state"> NONE, Initialized, Casting, Cancelled, Deactivated, Activated, Destroyed</param>
    /// <param name="invokeAvailable">Permission, which provides to invoke events (In base case
    /// you can invoke events in server or is owner client. </param>
    public abstract class Weapon : NetworkRecognizableComponent
    {
        public class WeaponEvent
        {
            private readonly ushort unityEventId;
            private readonly Action<ushort> callServerRpc;
            private readonly UnityEvent unityEvent;
            public WeaponEvent(UnityEvent unityEvent, Action<ushort> callServerRpc, ushort unityEventId, bool isInvokeAvailable)
            {
                this.unityEvent = unityEvent;
                this.callServerRpc = callServerRpc;
                this.unityEventId = unityEventId;
            }
            public void AddListener(UnityAction a) => unityEvent.AddListener(a);
            public void RemoveListener(UnityAction a) => unityEvent.RemoveListener(a);
            public void Invoke()
            {
                callServerRpc(unityEventId);
            }
        }
        public class WeaponEvent<T>
        {
            private readonly ushort unityEventId;
            private readonly Action<T, ushort> callServerRpc;
            private readonly UnityEvent<T> unityEvent;
            private readonly bool isInvokeAvailable;
            public WeaponEvent(UnityEvent<T> unityEvent, Action<T, ushort> callServerRpc, ushort unityEventId, bool isInvokeAvailable)
            {
                this.unityEvent = unityEvent;
                this.callServerRpc = callServerRpc;
                this.unityEventId = unityEventId;
                this.isInvokeAvailable = isInvokeAvailable;
            }
            public void AddListener(UnityAction<T> a) => unityEvent.AddListener(a);
            public void RemoveListener(UnityAction<T> a) => unityEvent.RemoveListener(a);
            public void Invoke(T value)
            {
                if (isInvokeAvailable)
                {
                    callServerRpc(value, unityEventId);
                }
                else
                {
                    Debug.LogWarning("No permissions to invoke in this platform! Check Weapon.invokeAvailable");
                }
               
            }
        }

        [Header("Weapons components")]
        [SerializeField]
        protected WeaponDesign weaponDesign;
        
        [Range(0, 100)] 
        protected int power = 100;
        
        public WeaponState state { get; private set; } 
        
        protected virtual bool invokeAvailable => IsServer || (IsClient && IsOwner && !IsServer);
        
        #region Events

        /// <summary>
        /// This event invokes, when player starts to cast the weapon
        /// </summary>
        protected WeaponEvent CastStartedEvent { get; private set; }
        
        /// <summary>
        /// This event invokes, when player cancel to cast the weapon
        /// </summary>
        protected WeaponEvent CastCancelledEvent { get; private set; }
        
        /// <summary>
        /// This event invokes, when all frames was recognized
        /// </summary>
        protected WeaponEvent GestureCastedEvent { get; private set; }
       

        /// <summary>
        /// This event invokes, when the player casts Gesture and takes/grabs/casts weapon (it's able to shoot)
        /// </summary>
        protected WeaponEvent ActivatedEvent { get; private set; }

        /// <summary>
        /// This function is called when the player throws/loses the weapon
        /// </summary>
        protected WeaponEvent DeactivatedEvent { get; private set; }
    
        /// <summary>
        /// <example>
        /// Sword speed more than 5, gesture palm detected, someone near of the player
        /// </example>
        /// This function describes the start of hit 
        /// </summary>
        protected WeaponEvent StartHitEvent { get; private set; }

        /// <summary>
        /// <example>
        /// Sword speed less than 5, gesture palm is not recognized, someone too far from the player
        /// </example>
        /// This function describes the end of hit
        /// </summary>
        protected WeaponEvent StopHitEvent { get; private set; }

        /// <summary>
        /// <example>
        /// Sword crashed, bullets count == 0, delay ended
        /// </example>
        /// This function describes the destroy of ability
        /// </summary>
        protected WeaponEvent AbilityDestroyedEvent { get; private set; }


        /// <summary>
        /// This event invokes, when the frame was recognized
        /// </summary>
        protected WeaponEvent<string> FrameRecognizedEvent { get; private set; }
        
        /// <example>
        /// bullet inside the target, player in hammer-hit area, the magic spell has found the enemy
        /// </example>
        /// This function describes the hit impact ability.
        /// <returns>UnityEvent of type Affected for handling impact events.</returns>
        protected WeaponEvent<string> ImpactEvent { get; private set; }

      
        private UnityEvent _CastStarted;
        private UnityEvent _CastCancelled;
        private UnityEvent _GestureCasted;
        private UnityEvent _Activated;
        private UnityEvent _Deactivated;
        private UnityEvent _StartHit;
        private UnityEvent _StopHit;
        private UnityEvent _AbilityDestroyed;
        
        private UnityEvent<string> _FrameRecognized;
        private UnityEvent<string> _Impact;
        
        private WeaponEvent[] _weaponEvents;
        private WeaponEvent<string>[] _weaponParamEvents;
        private UnityEvent[] _unityEvents;
        private UnityEvent<string>[] _unityParamEvents;
        #endregion

        #region RpcCalls
        
        // Retranslators - provide the synchronization of event between platforms
        [ClientRpc]
        private void CallEventClientRpc(ushort eventId)
        {
            _unityEvents[eventId]?.Invoke();
        }
        [ClientRpc]
        private void CallEventClientRpc(string value, ushort eventId)
        {
            _unityParamEvents[eventId]?.Invoke(value);
        }
        [ServerRpc]
        private void CallEventServerRpc(ushort eventId)
        {
            _unityEvents[eventId]?.Invoke();
        }
        [ServerRpc]
        private void CallEventServerRpc(string value, ushort eventId)
        {
            _unityParamEvents[eventId]?.Invoke(value);
        }

        

        #endregion
    
        private void SubscribeEvents() 
        {
        _weaponEvents = new[]
            {
                CastStartedEvent,
                CastCancelledEvent,
                GestureCastedEvent,
                ActivatedEvent,
                DeactivatedEvent,
                StartHitEvent,
                StopHitEvent,
                AbilityDestroyedEvent
            };
            _weaponParamEvents = new[] { FrameRecognizedEvent, ImpactEvent };
            _unityEvents = new[]
            {
                _CastStarted,
                _CastCancelled,
                _GestureCasted,
                _Activated,
                _Deactivated,
                _StartHit,
                _StopHit,
                _AbilityDestroyed
            };
   
            
            _unityParamEvents = new[] { _FrameRecognized, _Impact };
            try
            {
                for (ushort i = 0; i < _weaponEvents.Length; i++)
                {
                    _weaponEvents[i] = new WeaponEvent(_unityEvents[i], CallEventServerRpc, i, invokeAvailable);
                    if (IsClient)
                    {
                        _unityEvents[i].AddListener(weaponDesign.actions[i]);
                    }
                }

                for (ushort i = 0; i < _weaponParamEvents.Length; i++)
                {
                    _weaponParamEvents[i] =
                        new WeaponEvent<string>(_unityParamEvents[i], CallEventServerRpc, i, invokeAvailable);
                    if (IsClient)
                    {
                        _unityParamEvents[i].AddListener(weaponDesign.paramActions[i]);
                    }
                }

            }
            catch
            {
                state = WeaponState.NONE;
                Debug.LogError("The weapon events and unity events are different!");
                return;
            }

            state = WeaponState.Initialized;
            _CastStarted.AddListener(() => { state = WeaponState.Casting;});
            _CastCancelled.AddListener(() => { state = WeaponState.Cancelled;});
            _Activated.AddListener(() => { state = WeaponState.Activated; });
            _Deactivated.AddListener(() => { state = WeaponState.Deactivated; });
            _AbilityDestroyed.AddListener(() => { state = WeaponState.Destroyed;});
        }

        public sealed override void OnFrameRecognized(string name)
        {
            FrameRecognizedEvent?.Invoke(name);
        }

        public sealed override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            SubscribeEvents();
        }

        protected override void OnAbilityReleased()
        {
            AbilityDestroyedEvent?.Invoke();
        }

        public override void OnGestureCasted()
        {
            DeactivatedEvent?.Invoke();
        }
    }
}