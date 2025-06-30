using System;
using System.Collections.Generic;
using System.Linq;
using Scrips.Components;
using Scripts.Abilities;
using Scripts.Characters;
using Scripts.Components;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.Network;
using Scripts.PlayerLogic;
using Scripts.Static;
using Scripts.Static.Definitions;
using Scripts.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using CharacterController = Scripts.Characters.CharacterController;


namespace Scripts.PlayerLogic
{
    // public class PlayerData
    // {
    //     public readonly ulong id;
    //     public readonly BodyAnchors bodyAnchors;
    //     public readonly PlayerHands hands;
    //     public readonly GesturesLibrary library;
    //     public static PlayerData local;
    //
    //     public PlayerData(ulong id, BodyAnchors bodyAnchors, PlayerHands hands, GesturesLibrary library)
    //     {
    //         this.id = id;
    //         this.bodyAnchors = bodyAnchors;
    //         this.hands = hands;
    //         this.gesturesLib = library;
    //     }
    // }

    [Serializable]
    public class PlayerData
    {
        [DisableInEditorMode]
        public ulong id;
        public BodyAnchors bodyAnchors;
        public PlayerHands hands;
        public AbilityController abilityController;
        public CharacterController characterController;
        [HideInEditorMode]
        public Rig rig;
        [BoxGroup("Events")]
        public UnityEvent onComponentsInitialized = new UnityEvent();
        [BoxGroup("Events")]
        public UnityEvent<PlayerMode> onPlayerModeChanged = new UnityEvent<PlayerMode>();
        [BoxGroup("Events")]
        public UnityEvent<RigType> onPlayerRigChanged = new UnityEvent<RigType>();
        
        public Character character => characterController.currentCharacter;
        public Avatar avatar => character.curAvatar;
        public GesturesLibrary gesturesLibrary => abilityController.gesturesLib;
        
        public static PlayerData local;
        public static readonly PlayerMode[] modesWithGestureRecognition =
            { PlayerMode.MENU, PlayerMode.DYED, PlayerMode.ACTIVE };
    }

  
    public class Player : SmartComponent
    {
        public bool isInitialized { get; set; }
        [BoxGroup("Runtime Settings")] [SerializeField][DisableInPlayMode]
        private bool isLocal;
        
        [BoxGroup("Runtime Settings")][SerializeField][EnumToggleButtons][ShowInInspector][OnValueChanged("ActivateRig")][Space()][DisableInPlayMode]
        private RigType _rigType;
        
        [BoxGroup("Runtime Settings")][SerializeField][EnumToggleButtons][ShowInInspector][Space()][DisableInPlayMode]
        private PlayerMode _playerMode;
        
        [BoxGroup("Runtime Settings")][EnumToggleButtons][SerializeField][Space()][OnValueChanged("ChangeAvatarFromInspector")]
        public AvatarType debugAvatar;
        
        [BoxGroup("Runtime Settings")][SerializeField][ShowInInspector][Space()][DisableInPlayMode]
        public CharacterType debugCharacter;

        [SerializeField]
        private PlayerData _data;
        [SerializeField] 
        private Rig[] _rigList;
        
        private Dictionary<RigType, Rig> _rigDict = new ();
        
        
        public PlayerMode playerMode
        {
            get => _playerMode;
            set
            {
                _playerMode = value;
                data.onPlayerModeChanged?.Invoke(_playerMode);
            }
        }

        public RigType rigType
        {
            get => _rigType;
            set
            {
                _rigType = value;
                ActivateRig();
             
            }
        }
        public PlayerData data => _data;
        private void ActivateRig()
        {
            foreach (var rig in _rigList)
            {
                rig.gameObject.SetActive(_rigType == rig.type);
  
                if (rig.type == _rigType && Application.isPlaying)
                {
                    rig.Initialize();
                    _data.rig = rig;
                }
            }
        }


        private void OnDisable()
        {
            data.rig.OnDisable();
        }

        private void Awake()
        {
          
            _rigDict = new Dictionary<RigType, Rig>();
            foreach (var VARIABLE in _rigList)
            {
                if (_rigDict.ContainsKey(VARIABLE.type))
                    continue;
                _rigDict.Add(VARIABLE.type, VARIABLE);
            }

            if (isLocal)
            {
                SetOwner(0);
                LocalInitializing();
            }
            
        }
        
        public void SetOwner(ulong id)
        {
            PlayerData.local = data;
            InitializeComponents(id);
#if UNITY_EDITOR
            this.rigType = _rigType;
#elif PLATFORM_ANDROID
            rigType = RigType.OVRRig;
#else
            rigType = _rigType;
#endif
          
            data.abilityController.CreateRecognizer(data.rig.RecognitionPropertiesConfig);
        }

        public void LocalInitializing()
        {
            data.characterController.SpawnCharacters();
            data.characterController.SetCharacter(debugCharacter.ToString());
            data.characterController.SetAvatarType(debugAvatar);
        }

        public void SetEnemy(ulong id)
        {
            InitializeComponents(id);
            rigType = RigType.NoRig;
 
            data.characterController.SetAvatarType(AvatarType.Enemy);
        }

        private void InitializeComponents(ulong id)
        {
            data.characterController.SetMaterialId((int)id);
            data.abilityController.Initialize();
            
            AddLoggers();
            
            Global.updateEvent.AddListener(UpdateAnchors);
            data.onComponentsInitialized.AddListener(()=>
            {
                isInitialized = true;
            });
            data.abilityController.gesturesLib.onLibraryInitialized += data.onComponentsInitialized.Invoke;
           
            
            Debug.Log($"Player {id} initialized.");
        }
        private bool isAnyNull()
        {
            var allRigTypes = Enum.GetValues(typeof(RigType)).Cast<RigType>().ToList();

            var existingRigTypes = _rigDict.Keys.ToList();

            bool allRigTypesExist = allRigTypes.All(rt => existingRigTypes.Contains(rt));
            if (!allRigTypesExist)
            {
                Debug.Log("Please, add all avatars and rigs to player " + name);
                return true;
            }

            return false;
        }

        protected void UpdateAnchors()
        {
            if (!isInitialized)
                return;
                
            if (_rigType != RigType.NoRig)
            {
                // updating 
                BodyAnchors.EquateAnchors(data.rig.anchors,
                    ref data.bodyAnchors); // нельзя прокинуть _anchors в риг напрямую, потому-что в риге находится камера.
            }

            if (data.avatar)
                BodyAnchors.EquateAnchors(data.bodyAnchors, ref data.avatar.Anchors);
        }

        protected override bool shouldAddMissingComponents =>
            !(data.characterController && data.bodyAnchors && data.hands && data.abilityController);

        public override void AddMissingComponents()
        {
            data.characterController = GetComponentInChildren<CharacterController>();
            data.bodyAnchors = transform.Find("Anchors").GetComponent<BodyAnchors>();
            data.bodyAnchors.AddMissingComponents();
            data.hands = data.bodyAnchors.transform.GetComponentInChildren<PlayerHands>();
            foreach (var VARIABLE in _rigList)
            {
                VARIABLE.AddMissingComponents();
            }

            data.abilityController = transform.Find("AbilityController").GetComponent<AbilityController>();

            if (!isLocal)
            {
                Calculations.AddComponentSmart<NetworkPlayerProcessor>(transform);

                List<ClientTransform> transforms = new()
                {
                    Calculations.AddComponentSmart<ClientTransform>(data.bodyAnchors.Body),
                    Calculations.AddComponentSmart<ClientTransform>(data.bodyAnchors.Head),
                    Calculations.AddComponentSmart<ClientTransform>(data.hands.rightHand.points[0]),
                    Calculations.AddComponentSmart<ClientTransform>(data.hands.leftHand.points[0]),
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
            data.onPlayerModeChanged.AddListener(EventLogger.OnPlayerModeChanged);
            data.characterController.characterChangedEvent.AddListener(EventLogger.OnCharacterChanged);
        }

        public void ChangeAvatarFromInspector()
        {
            if(Application.isPlaying)
                data.characterController.SetAvatarType(debugAvatar);
        }
        
    }
}