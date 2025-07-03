using Components;
using Gesture_Editor_SDK.Realtime;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using Scripts.Players;
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
    
    /// <param name="ReadyToBeCastedEvent">Invokes automatically, when the weapon might be casted (by gameplay) - do reset functions here. Like mana = 100, setActive(false), position = (0,0,0)</param>
    /// <param name="CastCancelledEvent">Invokes automatically, or by implementation - when player spent a lot of time for cast</param>
    /// <param name="GestureCastedEvent">Invokes automatically when dynamic gesture recognized</param>
    /// <param name="ActivatedEvent">Must be invoked, when you want to activate it (Grab, Take, Push the button)</param>
    /// <param name="DeactivatedEvent">Must be invoked, when you want to deactivate it (Throw, Put, Push the button)</param>
    /// <param name="HitStartedEvent">Must be invoked, when the weapon is possible to take damage (katana speed > 0, switch turn on, spell sayed)</param>
    /// <param name="HitStoppedEvent">Must be invoked, when the weapons is not possible to take damage (katana speed  == 0, switch turn off) </param>
    /// <param name="AbilityDestroyedEvent">Must be invoked, when the weapon should be destroyed</param>
    /// <param name="FrameRecognizedEvent<string>">Invokes automatically, when the frame for cast is recognized</param>
    /// <param name="ImpactEvent<string>">Must be invoked, when the weapon hit someone/something. string - affected</param>

    /// <param name="state"> Sets automatically, by calling Events. States: NONE, Initialized, Casting, Cancelled, Deactivated, Activated, Destroyed</param>
    /// <param name="invokeAvailable">Permission, which provides to invoke events (In base case
    /// you can invoke events in server or is owner client. </param>
    public class Weapon : NetworkRecognizableComponent
    {

        [Header("Weapons components")]
        [SerializeField]
        protected WeaponDesign weaponDesign;
        public WeaponState state { get; private set; } 
        
        protected virtual bool invokeAvailable => IsServer || (IsClient && IsOwner);

        protected bool isSubscribed { get; private set; }
        public override string abilityName => transform.name.Split('_')[0];
        public override AbilityType type => AbilityType.Character;
        #region Events

        /// <summary>
        /// This event invokes, when player starts to cast the weapon
        /// </summary>
        protected WeaponEvent ReadyToBeCastedEvent { get; private set; }
        
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
        
        #region UnityEvents
        
        private readonly UnityEvent _ReadyToBeCasted = new UnityEvent();
        private readonly UnityEvent _CastCancelled = new UnityEvent();
        private readonly UnityEvent _GestureCasted = new UnityEvent();
        private readonly UnityEvent _Activated = new UnityEvent();
        private readonly UnityEvent _Deactivated = new UnityEvent();
        private readonly UnityEvent _StartHit = new UnityEvent();
        private readonly UnityEvent _StopHit = new UnityEvent();
        private readonly UnityEvent _AbilityDestroyed = new UnityEvent();

        private readonly UnityEvent<string> _FrameRecognized = new UnityEvent<string>();
        private readonly UnityEvent<string> _Impact = new UnityEvent<string>();
        
        private WeaponEvent[] _weaponEvents;
        private WeaponEvent<string>[] _weaponParamEvents;
        private UnityEvent[] _unityEvents;
        private UnityEvent<string>[] _unityParamEvents;
        #endregion
        
        #endregion

        #region RpcCalls
        
        // Retranslators - provide the synchronization of event between platforms
        [ClientRpc]
        private void CallEventClientRpc(ushort eventId)
        {
            if (IsServer)
                return;
            _unityEvents[eventId]?.Invoke();
            Debug.Log("CallEventClientRpc(ushort eventId");
        }
        [ClientRpc]
        private void CallEventClientRpc(string value, ushort eventId)
        {
            if (IsServer)
                return;
            _unityParamEvents[eventId]?.Invoke(value);
            Debug.Log($"CallEventClientRpc(string {value}, ushort eventId");
        }
        [ServerRpc]
        private void CallEventServerRpc(ushort eventId)
        {
            _unityEvents[eventId]?.Invoke();
            CallEventClientRpc(eventId);
            Debug.Log("CallEventServerRpc(ushort eventId)");
        }
        [ServerRpc]
        private void CallEventServerRpc(string value, ushort eventId)
        {
            _unityParamEvents[eventId]?.Invoke(value);
            CallEventClientRpc(value, eventId);
            Debug.Log($"CallEventClientRpc(string {value}, ushort eventId");
        }

        

        #endregion

        protected virtual void OnInitialized()
        {
        }
        
        private void SubscribeEvents() 
        {
            _weaponEvents = new WeaponEvent[8];
            _weaponParamEvents = new WeaponEvent<string>[2];
            _unityEvents = new []
            {
                _ReadyToBeCasted,
                _CastCancelled,
                _GestureCasted,
                _Activated,
                _Deactivated,
                _StartHit,
                _StopHit,
                _AbilityDestroyed
            };
            _unityParamEvents = new [] { _FrameRecognized, _Impact };
            
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
                    _weaponParamEvents[i] = new WeaponEvent<string>(_unityParamEvents[i], CallEventServerRpc, i, invokeAvailable);
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

            ReadyToBeCastedEvent = _weaponEvents[0];
            CastCancelledEvent = _weaponEvents[1];
            GestureCastedEvent = _weaponEvents[2];
            ActivatedEvent = _weaponEvents[3];
            DeactivatedEvent = _weaponEvents[4];
            StartHitEvent = _weaponEvents[5];
            StopHitEvent = _weaponEvents[6];
            AbilityDestroyedEvent = _weaponEvents[7];
            FrameRecognizedEvent = _weaponParamEvents[0];
            ImpactEvent = _weaponParamEvents[1];
            
            
            state = WeaponState.Initialized;
            _FrameRecognized.AddListener((e) => { state =  WeaponState.Casting;});
            _CastCancelled.AddListener(() => { state = WeaponState.Cancelled;});
            _Activated.AddListener(() => { state = WeaponState.Activated; });
            _Deactivated.AddListener(() => { state = WeaponState.Deactivated; });
            _AbilityDestroyed.AddListener(() => { state = WeaponState.Destroyed;});
            _AbilityDestroyed.AddListener( () =>{ AbilityReleasedEvent.Invoke(); });
            isSubscribed = true;
        }

        
    
        public override void Initialize(PlayerData data, DynamicGesture gesture)
        {
            base.Initialize(data, gesture);
            SubscribeEvents();
            weaponDesign.playerData = data;
            OnInitialized();
            InitializeClientRpc();
        }

        [ClientRpc]
        private void InitializeClientRpc()
        {
            var player = transform.parent.GetComponent<Player>();
            base.Initialize(player.data, null);
            SubscribeEvents();
            weaponDesign.playerData = player.data;
            OnInitialized();
            Debug.Log("[ClientRpc] Weapon " + name + ", id - " + this.NetworkObjectId + " intialized!");
        }

        public override void ReadyToBeRecognized()
        {
            ReadyToBeCastedEvent?.Invoke();
        }

        public sealed override void OnFrameRecognized(string name)
        {
            FrameRecognizedEvent?.Invoke(name);
        }
        public sealed override void OnGestureCasted()
        {
            GestureCastedEvent?.Invoke();
        }

        public override UnityEvent AbilityReleasedEvent { get; set; }

        public override void AddMissingComponents()
        {
            weaponDesign ??= GetComponent<WeaponDesign>();
        }
    }
}