using System;
using System.Collections.Generic;
using System.Linq;
using Scripts.Abilities;
using Scripts.Characters;
using Scripts.Components;
using Scripts.HandsLogic;
using Scripts.Network;
using Scripts.Players;
using Scripts.Static;
using Scripts.Static.Definitions;
using Scripts.Systems;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using CharacterController = Scripts.Characters.CharacterController;

namespace Scripts.PlayerLogic
{
    public class Player : SmartComponent
    {   
        
        [DisableInPlayMode][SerializeField]
        public PlayerProperties playerProperties;
        
        [BoxGroup("Runtime Settings")]
        public ulong id;
        [BoxGroup("Runtime Settings")] [SerializeField][DisableInPlayMode]
        private bool isLocal;
        
        [BoxGroup("Runtime Settings")] 
        [DisableInEditorMode][DisableInPlayMode]
        public bool isInitialized;
        
        [DisableInEditorMode][DisableInPlayMode]
        private PlayerMode _playerMode;

        [BoxGroup("Components")][SerializeField]
        private BodyAnchors anchors;
        
        [BoxGroup("Components")][SerializeField]
        private PlayerHands hands;
        
        [BoxGroup("Components")][SerializeField]
        private AbilityController abilityController;
        
        [BoxGroup("Components")][SerializeField]
        private CharacterController characterController;

        [BoxGroup("Events")]
        public UnityEvent onPlayerInitialized = new UnityEvent();
        
        [BoxGroup("Events")]
        public UnityEvent<PlayerMode> onPlayerModeChanged = new UnityEvent<PlayerMode>();
        
        [BoxGroup("Events")]
        public UnityEvent<RigType> onPlayerRigChanged = new UnityEvent<RigType>();

        [BoxGroup("Events")]
        public UnityEvent<HeadInteractionType> onHeadInteraction => _rig.headInteraction.onHeadInteraction;
        public PlayerData data => _data;
        private PlayerData _data;
        
        public Rig rig => _rig;
        [HideInEditorMode]
        private Rig _rig;
        
        [SerializeField] 
        private Rig[] _rigList;
        
        private Dictionary<RigType, Rig> _rigDict = new ();

        private Character character => isInitialized ? characterController.currentCharacter : null;
        private Avatar avatar => isInitialized ? character.curAvatar : null;
        
        public static readonly PlayerMode[] modesWithGestureRecognition =
            { PlayerMode.MENU, PlayerMode.DYED, PlayerMode.ACTIVE };
        
        public PlayerMode playerMode
        {
            get => _playerMode;
            set
            {
                _playerMode = value;
                onPlayerModeChanged?.Invoke(_playerMode);
            }
        }

        public RigType rigType
        {
            get => playerProperties.rig;
            set
            {
                playerProperties.rig = value;
                ActivateRig();
             
            }
        }

        private void ActivateRig()
        {
            foreach (var rig in _rigList)
            {
                rig.gameObject.SetActive(playerProperties.rig == rig.type);
  
                if (rig.type == playerProperties.rig && Application.isPlaying)
                {
                     _rig = rig;
                     if(isInitialized)
                        _rig.Initialize();
                     else
                         onPlayerInitialized.AddListener(_rig.Initialize);
                     
                }
            }
        }

        private void OnDisable()
        {
            _rig?.OnDisable();
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
                InitializePlayer(0, playerProperties);
                //CreateRecognizer();
            }
        }

        public void InitializePlayer(ulong _id, PlayerProperties _playerProps)
        {
            id = _id;
            AddLoggers();
            
            characterController.SpawnCharacters();
            abilityController.Initialize();
       
            characterController.SetMaterialId((int)_id);
            characterController.SetCharacter(_playerProps.character.ToString());
            characterController.SetAvatarType(_playerProps.avatar);
           
            
            rigType = _playerProps.rig;
            
            onPlayerInitialized.AddListener(()=> { isInitialized = true; });
            
            _data = new PlayerData(this, anchors, hands, abilityController, characterController, abilityController.gesturesLib);
            abilityController.gesturesLib.onLibraryInitialized += data.onPlayerInitialized.Invoke;
            foreach (var VARIABLE in _rigList)
            {
                if(VARIABLE.type == _playerProps.rig)
                    abilityController.CreateRecognizer(VARIABLE.RecognitionPropertiesConfig, hands);
            }
            data.onPlayerInitialized.AddListener(() =>
            {
                Debug.Log(
                    $"Player {id} initialized. rig - {this.playerProperties.rig}, character - {this.playerProperties.character}, avatar - {this.playerProperties.avatar}");
            });
    
            if (isLocal)
                PlayerData.local = data;
            Global.updateEvent.AddListener(UpdateAnchors);
            
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
                
            if (playerProperties.rig != RigType.NoRig)
            {
                // updating 
                BodyAnchors.EquateAnchors(_rig.anchors,
                    ref anchors); // нельзя прокинуть _anchors в риг напрямую, потому-что в риге находится камера.
            }

            if (avatar)
                BodyAnchors.EquateAnchors(anchors, ref avatar.Anchors);
        }

        protected override bool shouldAddMissingComponents =>
            !(characterController && anchors && hands && abilityController);

        public override void AddMissingComponents()
        {
            characterController = GetComponentInChildren<CharacterController>();
            anchors = transform.Find("Anchors").GetComponent<BodyAnchors>();
            anchors.AddMissingComponents();
            hands = anchors.transform.GetComponentInChildren<PlayerHands>();
            foreach (var VARIABLE in _rigList)
            {
                VARIABLE.AddMissingComponents();
            }

            abilityController = transform.Find("AbilityController").GetComponent<AbilityController>();

            if (!isLocal)
            {
                Calculations.AddComponentSmart<NetworkPlayerProcessor>(transform);

                List<ClientTransform> transforms = new()
                {
                    Calculations.AddComponentSmart<ClientTransform>(anchors.Body),
                    Calculations.AddComponentSmart<ClientTransform>(anchors.Head),
                    Calculations.AddComponentSmart<ClientTransform>(hands.rightHand.points[0]),
                    Calculations.AddComponentSmart<ClientTransform>(hands.leftHand.points[0]),
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
            onPlayerModeChanged.AddListener(EventLogger.OnPlayerModeChanged);
            characterController.characterChangedEvent.AddListener(EventLogger.OnCharacterChanged);
        }

        public PlayerData GetRawPlayerData()
        {
                return new PlayerData(this, anchors, hands, abilityController, characterController, null);
            
        }
    }
}