using Scripts.Events;
using Scripts.Hands;
using Unity.Netcode;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    public enum RigType
    {
        PCRig,
        XRRig,
        NoRig,
    }
    public enum AvatarType
    {
        LocalAvatar,
        EnemyAvatar
    }

    public class Player : NetworkBehaviour
    {
        [Header("Runtime Settings")]
        
        [SerializeField] private RigType _rigType;
        [SerializeField] private AvatarType _avatarType;
        
        [Header("Avatars")]
        
        [SerializeField] private Avatar _localAvatar;
        [SerializeField] private Avatar _enemyAvatar;
        private Avatar _curAvatar;
        
        [Header("Rigs")]
        
        [SerializeField] private PlayerRig _pcRig;
        [SerializeField] private PlayerRig _xrRig;
        private PlayerRig _curRig;

        [Header("Anchors")] 
        
        [SerializeField] private BodyAnchors _anchors;

       // private UpdateEvent _onUpdate;
       private bool _isSynchronized;
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
        public AvatarType AvatarType
        {
            get => _avatarType;
            set
            {
                _avatarType = value;
                CurAvatar = GetAvatar();
                ActivateAvatar();
            }
        }
        public BodyAnchors Anchors => _anchors;
        private PlayerRig CurRig
        {
            get=>_curRig;
            set
            {
                _curRig = value;
                ActivateRig();
            }
        }
        public Avatar CurAvatar 
        {
            get=>_curAvatar;
            set
            {
                _curAvatar = value;
                ActivateAvatar();
            }
        }
        private Avatar GetAvatar()
        {
            switch (_avatarType)
            {
                case AvatarType.LocalAvatar:
                    return _localAvatar;
                case AvatarType.EnemyAvatar:
                    return _enemyAvatar;
                default:
                    return _localAvatar;
            }
        }
        private PlayerRig GetRig()
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
        protected void ActivateAvatar()
        {
            _localAvatar.gameObject.SetActive(false);
            _enemyAvatar.gameObject.SetActive(false);
            CurAvatar.gameObject.SetActive(true);
        }
    
        protected void ActivateRig()
        {
            _pcRig.gameObject.SetActive(false);
//            _xrRig.gameObject.SetActive(false);
            if(_rigType != RigType.NoRig)
                CurRig.gameObject.SetActive(true);
        }
        
        
        protected void OnValidate()
        {
            CurRig = GetRig();
            CurAvatar = GetAvatar();
            ActivateAvatar();
        }
        public void Construct(UpdateEvent onUpdate)
        {
         //   _onUpdate = onUpdate;
            if (IsOwner && IsClient)
            {
                _pcRig.movement.Construct(onUpdate);
                _xrRig.movement.Construct(onUpdate);
            }
        }
        
        public override void OnNetworkSpawn()
        {
            Debug.Log("NETWORK SPAWN");
            transform.name = $"Player {OwnerClientId}";
            if (IsClient && !IsOwner)
            {
                RigType = RigType.NoRig;
                AvatarType = AvatarType.EnemyAvatar;
            }

            if (IsClient && IsOwner)
            {
                RigType = RigType.PCRig;
                AvatarType = AvatarType.LocalAvatar;
            }

            if (IsServer)
            {
                RigType = RigType.NoRig;
                AvatarType = AvatarType.LocalAvatar;
            }
        }

        
        private void Start()
        {

            if (_rigType != RigType.NoRig)
            {
                _curRig.movement.StartMove();
            }
        
            StartWatch();
        }
        
        public void StartWatch()
        {
            _isSynchronized = true;
            // _onUpdate?.AddListener(UpdateTransforms);
        }
        public void StopWatch()
        {
            _isSynchronized = false;
            // _onUpdate?.RemoveListener(UpdateTransforms);
        }

        private void Update()
        {
            if (_isSynchronized)
            {
                UpdateTransforms();
            }
        }

        private void UpdateTransforms()
        {
            if (_rigType != RigType.NoRig)
            {
                _anchors.Head.position = CurRig.GetHead().position;
                _anchors.Head.rotation = CurRig.GetHead().rotation;
                _anchors.Body.position = CurRig.GetBody().position;
                _anchors.Body.rotation = CurRig.GetBody().rotation;
            }
            
            CurAvatar.head.position = _anchors.Head.position;
            CurAvatar.head.rotation = _anchors.Head.rotation;
            CurAvatar.body.position = _anchors.Body.position;
            CurAvatar.body.rotation = _anchors.Body.rotation;
        }
    }
}
