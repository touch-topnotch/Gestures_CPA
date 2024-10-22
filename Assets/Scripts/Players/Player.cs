using Unity.Netcode;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    public enum RuntimeRig
    {
        PCRig,
        XRRig,
        NoRig,
    }
    public enum RuntimeAvatar
    {
        LocalAvatar,
        EnemyAvatar
    }

    public class Player : NetworkBehaviour
    {
        [Header("Runtime Settings")]
        
        [SerializeField] private RuntimeRig _runtimeRig;
        [SerializeField] private RuntimeAvatar _runtimeAvatar;
        
        [Header("Avatars")]
        
        [SerializeField] private Avatar _localAvatar;
        [SerializeField] private Avatar _enemyAvatar;
        private Avatar _curAvatar;
        
        [Header("Rigs")]
        
        [SerializeField] private VRRig _pcRig;
        [SerializeField] private VRRig _xrRig;
        private VRRig _curRig;

        [Header("Anchors")] 
        
        [SerializeField] private BodyAnchors _anchors;
        
        public RuntimeRig RuntimeRig
        {
            get => _runtimeRig;
            set
            {
                _runtimeRig = value;
                CurRig = GetRig();
                ActivateRig();
            }

        }
        public RuntimeAvatar RuntimeAvatar
        {
            get => _runtimeAvatar;
            set
            {
                _runtimeAvatar = value;
                CurAvatar = GetAvatar();
                ActivateAvatar();
            }
        }
        public BodyAnchors Anchors => _anchors;
        private VRRig CurRig
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
            switch (_runtimeAvatar)
            {
                case RuntimeAvatar.LocalAvatar:
                    return _localAvatar;
                case RuntimeAvatar.EnemyAvatar:
                    return _enemyAvatar;
                default:
                    return _localAvatar;
            }
        }
        private VRRig GetRig()
        {
            switch (_runtimeRig)
            {
                case RuntimeRig.XRRig:
                    return _xrRig;
                case RuntimeRig.PCRig:
                    return _pcRig;
                case RuntimeRig.NoRig:
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
            _xrRig.gameObject.SetActive(false);
            CurRig.gameObject.SetActive(true);
        }
        
        
        protected void OnValidate()
        {
            CurRig = GetRig();
            CurAvatar = GetAvatar();
            _anchors.Body = this.transform;
        } 
        
        public override void OnNetworkSpawn()
        {
            Debug.Log("NETWORK SPAWN");
            if (IsClient && !IsOwner)
            {
                RuntimeRig = RuntimeRig.NoRig;
                RuntimeAvatar = RuntimeAvatar.EnemyAvatar;
            }

            if (IsClient && IsOwner)
            {
                RuntimeRig = RuntimeRig.PCRig;
                RuntimeAvatar = RuntimeAvatar.LocalAvatar;
            }

            if (IsServer)
            {
                RuntimeRig = RuntimeRig.NoRig;
                RuntimeAvatar = RuntimeAvatar.LocalAvatar;
            }
        }
    }
}
 