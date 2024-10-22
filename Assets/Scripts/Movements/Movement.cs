using Scripts.Events;
using Scripts.PlayerLogic;
using Scripts.Static;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Zenject;

namespace Scripts.Movements
{
    public abstract class Movement : MonoBehaviour
    {
        [SerializeField] protected PlayerRig anchors;
        [SerializeField] protected bool moveOnAwake;
        protected CharacterController parentMoveController;
        
        private UpdateEvent _onUpdate;
        private bool _isMoved = false;
        
        [Inject]
        protected virtual void Construct(UpdateEvent onUpdate)
        {
            parentMoveController = anchors.GetBody().GetComponent<CharacterController>();
            _onUpdate = onUpdate;
            if(moveOnAwake)
                StartMove();
        }

        public virtual void StartMove()
        {
            if (_onUpdate == null)
            {
                moveOnAwake = true;
                return;
            }

            if (_isMoved)
                return;
            
            _onUpdate.AddListener(UpdateVelocity);
            _isMoved = true;
            
            Debug.Log("Movement started");
        }

        public virtual void StopMove()
        {
            if(!_isMoved)
                return;
            
            _onUpdate.RemoveListener(UpdateVelocity);
            _isMoved = false;
            
            Debug.Log("Movement stopped");
        }
        

        protected virtual void UpdateVelocity()
        {
        }

   
    }
}