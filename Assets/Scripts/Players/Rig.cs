using Scripts.Events;
using Scripts.Gestures;
using Scripts.Hands;
using Scripts.Movements;
using UnityEngine;

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
        [SerializeField] protected BodyAnchors anchors;
        [SerializeField] private RecognitionPropertiesConfig recognitionProperties;
        
        protected PlayerStateChangedEvent playerStateChangedEvent;
        protected PlayerState playerState;
        public PlayerHands Hands => hands;
        public BodyAnchors Anchors => anchors;
        public RecognitionPropertiesConfig RecognitionPropertiesConfig => recognitionProperties;
    
 

        protected virtual void Start()
        {
            
            if (!transform.gameObject.activeSelf)
                return;
            
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