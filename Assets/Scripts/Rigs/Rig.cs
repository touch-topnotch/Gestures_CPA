using Scrips.Components;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.Movements;
using Scripts.Systems;
using Sirenix.OdinInspector;
using UnityEditor;
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

    public abstract class Rig : PlayerComponent, IMovable
    {
        [Header("Rig Components")]
        public BodyAnchors anchors;
      
        [SerializeField]
        private HeadInteraction _headInteraction;
        
        [SerializeField]
        private RecognitionPropertiesConfig _recognitionProperties;
    
        protected Hands hands => inherited.data.hands;
        protected PlayerStateChangedEvent playerStateChangedEvent;
        protected PlayerState playerState;
        public HeadInteraction headInteraction => _headInteraction;

        public RecognitionPropertiesConfig RecognitionPropertiesConfig => _recognitionProperties;

        protected void OnValidate()
        {
            if(anchors && anchors.Head)
                _headInteraction = anchors.Head.GetComponent<HeadInteraction>();
        }
        
        public virtual void Initialize()
        {
            if (!transform.gameObject.activeSelf)
                return;

            playerStateChangedEvent = new PlayerStateChangedEvent();
            playerStateChangedEvent.AddListener(OnPlayerStateChanged);
            headInteraction.onHeadInteraction += (headInteractionType) =>
            {
                Debug.Log("Recognized " + headInteractionType);
            };
        }

        protected virtual void OnPlayerStateChanged(PlayerState state)
        {
            Debug.Log("Current state: " + state);
        }

        public abstract bool isMoved();
        public abstract void StartMove();
        public abstract void StopMove();

        protected abstract void Centrize();
        
        public override void AddMissingComponents()
        {
            var rig = this.gameObject;
            anchors ??= rig.GetComponentInChildren<BodyAnchors>();
            _headInteraction ??= rig.GetComponentInChildren<HeadInteraction>();
        }

        protected override bool shouldAddMissingComponents =>
            !(anchors && _recognitionProperties && _headInteraction);

    }
}