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
      
        protected CharacterController parentMoveController;
        [SerializeField] protected float gravity;
        
        private UpdateEvent _onUpdate;
        
        private bool _isMoved;
        private bool _waitToConstruct;
   
        public void Construct(ref UpdateEvent onUpdate)
        {
            _onUpdate = onUpdate;
            parentMoveController = anchors.GetBody().GetComponent<CharacterController>();
            if (_waitToConstruct)
            {
                StartMove();
            }
        }

        public virtual void StartMove()
        {
            Debug.Log("Movement try to start...");
            if (_onUpdate == null)
            {
                Debug.Log("Movement can't start. Update Event == null");
                _waitToConstruct = true;
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
        


        protected abstract void UpdateVelocity();


    }
}