using System;
using System.Collections.Generic;
using Gesture_Editor_SDK.EditorAttributes.InspectorButtonAttribute;
using Scripts.Characters;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.Network;
using Scripts.Static;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;


namespace Scripts.PlayerLogic
{
    public enum RigType
    {
        PCRig,
        XRRig,
        NoRig,
    }
    public struct PlayerData
    {
        public readonly ulong id;
        public readonly Transform transform;
        public readonly PlayerHands hands;
        public PlayerData(ulong id, Transform transform, PlayerHands hands)
        {
            this.id = id;
            this.transform = transform;
            this.hands = hands;
        }
    }

 

    public class Player : MonoBehaviour
    {
        [Header("Runtime Settings")] 
        [SerializeField] private RigType _rigType;

        [FormerlySerializedAs("_characterController")] [SerializeField] private CharacterPool _characterPool;

        [SerializeField] private bool isLocal;

        private GestureCombiner _gestureCombiner;

        [Header("Rigs")] [SerializeField] private PCRig _pcRig;
        [SerializeField] private XRRig _xrRig;
        [SerializeField] private Rig _curRig;
        

        [Header("Anchors")] 
        [SerializeField] private BodyAnchors _anchors;

        [SerializeField] private PlayerHands _hands;
        
        
        public PlayerData data;
        public GestureCombiner gestureCombiner => _gestureCombiner;
        public BodyAnchors anchors => _anchors;
        public Character character => _characterPool.CurrentCharacter;
        public CharacterPool characterPool => _characterPool;
        public RigType RigType
        {
            get => _rigType;
            set
            {
                _rigType = value;
                CurRig = GetRig();
                ActivateRig();
            }

        }

        public Rig CurRig
        {
            get => _curRig;
            set
            {
                _curRig = value;
                if(_curRig != null) ActivateRig();
            }
        }

        private Rig GetRig()
        {
            switch (_rigType)
            {
                case RigType.XRRig:
                    return _xrRig;
                case RigType.PCRig:
                    return _pcRig;
                case RigType.NoRig:
                    return null;
                default:
                    return _pcRig;
            }
        }

        private void ActivateRig()
        {
            _pcRig.gameObject.SetActive(_rigType == RigType.PCRig);
            _xrRig.gameObject.SetActive(_rigType == RigType.XRRig);
        }
#if UNITY_EDITOR
        [Button("Add missing components")]
        private void AddMissingComponents()
        {
           

            _characterPool = this.GetComponentInChildren<CharacterPool>();
            _anchors = this.transform.Find("Anchors").GetComponent<BodyAnchors>();
            _anchors.Body = _anchors.transform.Find("Body");
            _anchors.Head = _anchors.Body.Find("Head");
            _hands = _anchors.transform.GetComponentInChildren<PlayerHands>();
            _pcRig = transform.Find("PC Rig").GetComponent<PCRig>();
            _xrRig = transform.Find("XR Rig").GetComponent<XRRig>();
            
            CurRig = GetRig();
            
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
#endif


        private void Awake()
        {
            data = new PlayerData(0, transform, _hands);
            _gestureCombiner = new GestureCombiner(data);
        }

        private void Start()
        {
            if(isLocal) Initialize();
        }

        public void Initialize()
        {
            
            if (_rigType != RigType.NoRig)
            {
                
                if (!_pcRig)
                    _pcRig = GetComponentInChildren<PCRig>();
                if (!_xrRig)
                    _xrRig = GetComponentInChildren<XRRig>();

                if (_rigType == RigType.PCRig)
                    _pcRig.library = _gestureCombiner.library;
                
                _gestureCombiner.CreateRecognizer(_curRig.RecognitionPropertiesConfig);
                
             //   _curRig.StartMove();
              
            }
            UpdateEvent.Instance.AddListener(UpdateAnchors); 
        }

        private bool isAnyNull()
        {
            if (_pcRig == null || _xrRig == null)
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
                BodyAnchors.EquateAnchors(_curRig.Anchors, _anchors); // нельзя прокинуть _anchors в риг напрямую, потому-что в риге находится камера.
            }

          
            BodyAnchors.EquateAnchors(_anchors, character.GetAvatar()?.Anchors);
        }
    }
}