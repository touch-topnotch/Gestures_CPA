using Scripts.Events;
using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.Movements;
using Scripts.Systems;
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
     
        [SerializeField] protected BodyAnchors anchors;
        [SerializeField] protected Hands hands;
        
        [SerializeField] private RecognitionPropertiesConfig _recognitionProperties;
        [SerializeField] private HeadInteraction _headInteraction;
 
        protected PlayerStateChangedEvent playerStateChangedEvent;
        protected PlayerState playerState;
        public HeadInteraction headInteraction => _headInteraction;
        public BodyAnchors Anchors => anchors;

        public RecognitionPropertiesConfig RecognitionPropertiesConfig => _recognitionProperties;
        protected virtual void Start()
        {
            
            if (!transform.gameObject.activeSelf)
                return;
            
            playerStateChangedEvent = new PlayerStateChangedEvent();
            playerStateChangedEvent.AddListener(OnPlayerStateChaned);
            headInteraction.onHeadInteraction += (headInteractionType) =>
            {
                Debug.Log("Recognized " + headInteractionType);
            };
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