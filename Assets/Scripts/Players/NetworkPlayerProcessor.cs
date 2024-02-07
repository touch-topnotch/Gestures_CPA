using Unity.Netcode;
using UnityEngine;

namespace Scripts.PlayerLogic
{
 

    [RequireComponent(typeof(Player))]
    public class NetworkPlayerProcessor : NetworkBehaviour
    {

        private Player _player;
        // private UpdateEvent _onUpdate;
        private bool _isSynchronized;

        private void Awake()
        {
            _player = GetComponent<Player>();
        }
        
        public override void OnNetworkSpawn()
        {
            Debug.Log("NETWORK SPAWN");
            transform.name = $"Player {OwnerClientId}";
            
            if (IsClient && !IsOwner)
            {
                _player.RigType = RigType.NoRig;
                _player.Character.CurrentType = AvatarType.Enemy;
                
            }

            if (IsClient && IsOwner)
            {
                _player.RigType = RigType.PCRig;
                _player.Character.CurrentType = AvatarType.Local;
            }

            if (IsServer)
            {
                _player.RigType = RigType.NoRig; 
                _player.Character.CurrentType = AvatarType.None;
            }
            Debug.Log("Changing player: " + _player.name + " to " + _player.Character.CurrentType);
            
            _player.Initialize();
        }
        private void Start()
        {
            StartWatch();
        }
        
        private  void StartWatch()
        {
            _isSynchronized = true;
            // _onUpdate?.AddListener(UpdateTransforms);
        }
        
        private void StopWatch()
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
            // _pLayer.CurAvatar.head.position = _anchors.Head.position;
            // _pLayer.CurAvatar.head.rotation = _anchors.Head.rotation;
            // _pLayer.CurAvatar.body.position = _anchors.Body.position;
            // _pLayer.CurAvatar.body.rotation = _anchors.Body.rotation;
        }
    }
}
