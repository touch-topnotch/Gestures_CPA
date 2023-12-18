using Scripts.Events;
using Scripts.Gestures;
using Scripts.Hands;
using Scripts.Movements;
using Scripts.Static;
using UnityEngine;
using Zenject;

namespace Scripts.PlayerLogic
{
    public enum PlayerState
    {
        MENU,
        ACTIVE,
        DYED,
        SPECTATOR
    }
    public abstract class Rig : MonoBehaviour
    {
        
        [SerializeField] protected PlayerHands hands;
        public PlayerHands GetHands => hands;
        
        
        [SerializeField] protected Movement movement;
        public Movement GetMovement => movement;
        
        
        [SerializeField] protected Transform head;
        public Transform GetHead => head;
        
        
        [SerializeField] protected Transform body;
        public Transform GetBody => body;
        
        protected PlayerStateChangedEvent playerStateChangedEvent;
        protected PlayerState playerState;

        
        [Inject]
        private void Construct(GesturesLibrary library)
        {
            if (!transform.gameObject.activeSelf)
                return;
      
            foreach (var gesture in library.DynamicGestures)
            {
                gesture.AddGraphicsToRigHands(hands);
            }
        }
        protected virtual void Start()
        {
            playerStateChangedEvent = new PlayerStateChangedEvent();
            playerStateChangedEvent.AddListener(OnPlayerStateChaned);
        }

        protected virtual void OnPlayerStateChaned(PlayerState state)
        {
            Debug.Log("Current state: " + state);
        }
    }
}