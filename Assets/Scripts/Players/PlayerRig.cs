using Scripts.Events;
using Scripts.Hands;
using UnityEngine;
using Zenject;

namespace Scripts.PlayerLogic
{
    public class PlayerRig: MonoBehaviour
    {

        [SerializeField] protected Transform head;

        [SerializeField] protected Transform body;

        protected BonesData left;
        protected BonesData right;

        protected UpdateEvent onUpdate;
        
        public Transform GetHead() => head;
        public Transform GetBody() => body;
        
        public BonesData GetLeft() => left;
        
        public BonesData GetRight() => right;

        [Inject]
        protected virtual void Construct(UpdateEvent _onUpdate)
        {
            onUpdate = _onUpdate;
        }
        

    }
}