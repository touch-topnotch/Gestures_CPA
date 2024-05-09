using System.Collections.Generic;
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
        XRRig,

        PCRig,

        NoRig,
    }

    public class PlayerData
    {
        public static PlayerData local;
        public readonly ulong id;
        public readonly BodyAnchors bodyAnchors;
        public readonly PlayerHands hands;
        public GesturesLibrary library;

        public PlayerData(ulong id, BodyAnchors bodyAnchors, PlayerHands hands, GesturesLibrary library)
        {
            this.id = id;
            this.bodyAnchors = bodyAnchors;
            this.hands = hands;
            this.library = library;
            local = this;
        }
    }


    public class Player : MonoBehaviour
    {
        [Header("Runtime Settings")] [SerializeField]
        private bool isLocal;

        [InspectorName("Debug Rig")] [SerializeField]
        private RigType _rigType;

        public RigType rigType
        {
            get => _rigType;
            set
            {
                Debug.Log("Rig type changed on " + value);
                _rigType = value;
                switch (value)
                {
                    case RigType.XRRig:
                        curRig = _xrRig;
                        break;
                    case RigType.PCRig:
                        curRig = _pcRig;
                        break;
                    case RigType.NoRig:
                        curRig = null;
                        break;
                }

                ActivateRig();
            }
        }

        [FormerlySerializedAs("_characterController")] [SerializeField]
        private CharacterPool _characterPool;


        [SerializeField] private GestureCombiner _gestureCombiner;

        [Header("Rigs")] [SerializeField] private PCRig _pcRig;
        [SerializeField] private XRRig _xrRig;

        public Rig curRig { get; private set; }


        [Header("Anchors")] [SerializeField] private BodyAnchors _anchors;

        [SerializeField] private PlayerHands _hands;


        public PlayerData data;
        public GestureCombiner gestureCombiner => _gestureCombiner;
        public BodyAnchors anchors => _anchors;
        public Character character => _characterPool.currentCharacter;
        public CharacterPool characterPool => _characterPool;


        private void ActivateRig()
        {
            _pcRig.gameObject.SetActive(_rigType == RigType.PCRig);
            _xrRig.gameObject.SetActive(_rigType == RigType.XRRig);

            if (_rigType == RigType.PCRig)
                _pcRig.Initialize(data);

            if (_rigType == RigType.XRRig)
                _xrRig.Initialize(data);
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
            _pcRig.AddMissingComponents();
            _xrRig = transform.Find("XR Rig").GetComponent<XRRig>();
            _xrRig.AddMissingComponents();
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
#endif


        private void Awake()
        {
            rigType = RigType.NoRig;
            if (isLocal)
            {
                _characterPool.SpawnCharacters();
                SetOwner(0);
            }
        }

        public void SetOwner(ulong id)
        {
            InitializeComponents(id);

            if (_rigType == RigType.NoRig)
                rigType = RigType.PCRig;
#if UNITY_EDITOR
            rigType = _rigType;
#elif PLATFORM_ANDROID
            rigType = RigType.XRRig;
#else
            rigType = _rigType;
#endif
            characterPool.SetAvatarType(AvatarType.Local);
            _gestureCombiner.CreateRecognizer(curRig.RecognitionPropertiesConfig);
            data.library.onLibraryInitialized += () => { _gestureCombiner.RecognizeWithAllGestures(); };
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
            data = new PlayerData(id, anchors, _hands, _gestureCombiner.library);

            characterPool.SetMaterialId((int)id);
            UpdateEvent.Instance.AddListener(UpdateAnchors);
            Debug.Log($"Player {id} initialized. RigType = {rigType}");
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
                BodyAnchors.EquateAnchors(curRig.Anchors,
                    _anchors); // нельзя прокинуть _anchors в риг напрямую, потому-что в риге находится камера.
            }

            BodyAnchors.EquateAnchors(_anchors, character.GetAvatar()?.Anchors);
        }
    }
}