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
    public abstract class Rig : MonoBehaviour, IMovable
    {
        
        [SerializeField] protected PlayerHands hands;
        public PlayerHands Hands => hands;
        public BodyAnchors anchors;
        
        [Header("Properties")]
        [Space]
        [SerializeField] private RecognitionPropertiesConfig recognitionProperties;
        public RecognitionPropertiesConfig RecognitionProperties => recognitionProperties;
        
        protected PlayerStateChangedEvent playerStateChangedEvent;
        protected PlayerState playerState;

        [Inject]
        private void Construct(GesturesLibrary library)
        {
            if (!transform.gameObject.activeSelf)
                return;
      
            foreach (var gesture in library.DynamicGestures.Values)
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

        public abstract bool isMoved();
        public abstract void StartMove();
        public abstract void StopMove();
    }
}