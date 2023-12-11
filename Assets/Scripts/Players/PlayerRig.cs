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
    public abstract class PlayerRig : MonoBehaviour
    {
        
        public PlayerHands hands;

        public Movement movement;

        protected PlayerState _state;
        public PlayerStateChangedEvent playerStateChangedEvent;
        
        [SerializeField] protected Transform head;
        [SerializeField] protected Transform body;
        
        public Transform GetHead() => head;
        public Transform GetBody() => body;

        [Inject]
        private void Construct(GesturesLibrary library)
        {
            if (!transform.gameObject.activeSelf)
                return;
            foreach (var gesture in library.DynamicGestures)
            {
                gesture.AddGraphics(hands);
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