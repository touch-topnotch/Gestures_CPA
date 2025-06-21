using System;
using System.Collections.Generic;
using System.Linq;
using Scrips.Components;
using Scripts.Characters;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.Network;
using Scripts.PlayerLogic;
using Scripts.Static;
using Scripts.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


namespace Scripts.PlayerLogic
{
    public enum PlayerMode
    {
        MENU,
        ACTIVE,
        DYED,
        RECORDING,
        SPECTATOR
    }
   
    public enum RigType
    {
        XRRig,

        PCRig,
        
        OVRRig,

        NoRig,
    }

    public class PlayerData
    {
        public readonly ulong id;
        public readonly BodyAnchors bodyAnchors;
        public readonly PlayerHands hands;
        public readonly GesturesLibrary library;
        public static PlayerData local;
        public PlayerData(ulong id, BodyAnchors bodyAnchors, PlayerHands hands, GesturesLibrary library)
        {
            this.id = id;
            this.bodyAnchors = bodyAnchors;
            this.hands = hands;
            this.library = library;
        }
    }


    public class Player : PlayerComponent
    {
        [Header("Runtime Settings")] [SerializeField]
        private bool isLocal;

        private PlayerMode _playerMode;
        [EnumToggleButtons]
        [ShowInInspector]
        public PlayerMode playerMode
        {
            get => _playerMode;
            set
            {
                _playerMode = value;
                onPlayerModeChanged.Invoke(_playerMode);
            }
        }
        
        [InspectorName("Debug Rig")] [SerializeField][EnumToggleButtons][OnValueChanged("ActivateRig")]
        private RigType _rigType;
        public RigType rigType
        {
            get => _rigType;
            set
            {
                Debug.Log("Rig type changed on " + value);
                _rigType = value;
                curRig = _rigDict[_rigType];
                ActivateRig();
            }
        }
        
        [SerializeField] private Rig[] _rigList;
        private Dictionary<RigType, Rig> _rigDict = new Dictionary<RigType, Rig>();
        
        [Header("Components")]
        [FormerlySerializedAs("_characterController")] [SerializeField]
        private CharacterPool _characterPool;
        [SerializeField]
        private GestureCombiner _gestureCombiner;
        public Rig curRig { get; private set; }

        [Header("Anchors")]
        [SerializeField] private BodyAnchors _anchors;

        [SerializeField] private PlayerHands _hands;

        [HideInInspector] public PlayerData data => _data ??= SetPlayerData();
        private PlayerData _data;
        public GestureCombiner gestureCombiner => _gestureCombiner;
        public BodyAnchors anchors => _anchors;
        public Character character => _characterPool.currentCharacter;
        public CharacterPool characterPool => _characterPool;

        public static readonly PlayerMode[] modesWithGestureRecognition = { PlayerMode.MENU, PlayerMode.DYED, PlayerMode.ACTIVE };
        public UnityEvent<PlayerMode> onPlayerModeChanged = new UnityEvent<PlayerMode>();
        public PlayerData SetPlayerData()
        {
            return new PlayerData(0, _anchors, _hands, _gestureCombiner?.library);
        }
        private void ActivateRig()
        {
            foreach (var rig in _rigList)
            {
                rig.gameObject.SetActive(_rigType == rig.type);
                if(rig.type == _rigType && Application.isPlaying)
                    rig.Initialize();
            }
        }

       
        
        private void Awake()
        {
            AddLoggers();
            _rigDict = new Dictionary<RigType, Rig>();
            foreach (var VARIABLE in _rigList)
            {
                if (_rigDict.ContainsKey(VARIABLE.type))
                    continue;
                _rigDict.Add(VARIABLE.type, VARIABLE);
            }
            if (isLocal)
            {
                _characterPool.SpawnCharacters();
                SetOwner(0);
            }
        }

        public void SetOwner(ulong id)
        {
            InitializeComponents(id);
            
            PlayerData.local = data;
            
#if UNITY_EDITOR
            rigType = _rigType;
#elif PLATFORM_ANDROID
            rigType = RigType.OVRRig;
#else
            rigType = _rigType;
#endif
            
            characterPool.SetAvatarType(AvatarType.Local);
            _gestureCombiner.CreateRecognizer(curRig.RecognitionPropertiesConfig);
            if (modesWithGestureRecognition.Contains(playerMode))
            {
                data.library.onLibraryInitialized += () => { _gestureCombiner.RecognizeWithAllGestures(); };
            }
        }

        public void SetEnemy(ulong id)
        {
            rigType = RigType.NoRig;
            InitializeComponents(id);
            characterPool.SetAvatarType(AvatarType.Enemy);
        }

        private void InitializeComponents(ulong id)
        {
            _gestureCombiner.Initialize(characterPool);
            _data = SetPlayerData(); 
            
            characterPool.SetMaterialId((int)id);
            UpdateEvent.Instance.AddListener(UpdateAnchors);
            Debug.Log($"Player {id} initialized. RigType = {rigType}");
        }


        private bool isAnyNull()
        {
            var allRigTypes = Enum.GetValues(typeof(RigType)).Cast<RigType>().ToList();

            var existingRigTypes = _rigDict.Keys.ToList();

            bool allRigTypesExist = allRigTypes.All(rt => existingRigTypes.Contains(rt));
            if(!allRigTypesExist)
            {
                Debug.Log("Please, add all avatars and rigs to player " + name);
                return true;
            }

            return false;
        }

        protected void UpdateAnchors()
        {
            if (_rigType != RigType.NoRig)
            {
                // updating 
                BodyAnchors.EquateAnchors(curRig.anchors,
                    ref _anchors); // нельзя прокинуть _anchors в риг напрямую, потому-что в риге находится камера.
            }
            if(character.curAvatar)
                BodyAnchors.EquateAnchors(_anchors, ref character.curAvatar.Anchors);
        }

        protected override bool shouldAddMissingComponents =>
            !(_characterPool && _anchors && _hands  && _gestureCombiner);

        public override void AddMissingComponents()
        {
            _characterPool = GetComponentInChildren<CharacterPool>();
            _anchors = transform.Find("Anchors").GetComponent<BodyAnchors>();
            _anchors.AddMissingComponents();
            _hands = _anchors.transform.GetComponentInChildren<PlayerHands>();
            foreach (var VARIABLE in _rigList)
            {
                VARIABLE.AddMissingComponents();
            }
            _gestureCombiner = transform.Find("GestureCombiner").GetComponent<GestureCombiner>();

            if (!isLocal)
            {
                Calculations.AddComponentSmart<NetworkPlayerProcessor>(transform);

                List<ClientTransform> transforms = new()
                {
                    Calculations.AddComponentSmart<ClientTransform>(anchors.Body),
                    Calculations.AddComponentSmart<ClientTransform>(anchors.Head),
                    Calculations.AddComponentSmart<ClientTransform>(_hands.rightHand.points[0]),
                    Calculations.AddComponentSmart<ClientTransform>(_hands.leftHand.points[0]),
                };

                foreach (var VARIABLE in transforms)
                {
                    VARIABLE.SyncPositionX = true;
                    VARIABLE.SyncPositionY = true;
                    VARIABLE.SyncPositionZ = true;
                    VARIABLE.SyncRotAngleX = true;
                    VARIABLE.SyncRotAngleY = true;
                    VARIABLE.SyncRotAngleZ = true;
                    VARIABLE.SyncScaleX = false;
                    VARIABLE.SyncScaleY = false;
                    VARIABLE.SyncScaleZ = false;
                    VARIABLE.InLocalSpace = true;
                    VARIABLE.Interpolate = true;
                    VARIABLE.SlerpPosition = true;
                }
            }
        }
        private void AddLoggers()
        {
            onPlayerModeChanged.AddListener(EventLogger.OnPlayerModeChanger);
        }
    }
}