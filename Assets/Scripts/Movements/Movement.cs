using System;
using System.Runtime.InteropServices;
using Scripts.Events;
using Scripts.Static;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Scripts.Movements
{
    public abstract class Movement : MonoBehaviour
    {
        [SerializeField] protected Transform ParentAnchor;
        protected CharacterController parentMoveController;

        public UpdateEvent OnUpdate;
        
        private bool _isStartedInConstruct = false;
        private bool _isMoved = false;
        
        [Inject]
        protected void Construct(UpdateEvent onUpdate)
        {
            Spawner.TryGetComponent(ParentAnchor,out parentMoveController);
            OnUpdate = onUpdate;
            if(_isStartedInConstruct)
                StartMove();
        }

        public virtual void StartMove()
        {
            if (OnUpdate == null)
            {
                _isStartedInConstruct = true;
                return;
            }

            if (_isMoved)
                return;
            OnUpdate.AddListener(UpdateVelocity);
            _isMoved = true;
            
            Debug.Log("Movement started");
        }

        public virtual void StopMove()
        {
            if(!_isMoved)
                return;
            
            OnUpdate.RemoveListener(UpdateVelocity);
            _isMoved = false;
            
            Debug.Log("Movement stopped");
        }

        protected virtual void UpdateVelocity()
        {
        }
    }
}