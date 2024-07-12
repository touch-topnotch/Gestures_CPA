using CrossPlatform.Scripts;
using UnityEngine;

namespace CrossPlatform.Movement
{
    public class DefaultMovement
    {
        [SerializeField]
        private Transform _heapAnchor;
        [SerializeField]
        private Transform _parentAnchor;
        private Vector3 _velocity;
        
        public void StartMove(ref OnUpdate onUpdate)
        {
            onUpdate += UpdateVelocity;
        }
        public void StopMove(ref OnUpdate onUpdate)
        {
            onUpdate -= UpdateVelocity;
        }
        private void UpdateVelocity()
        {
            _velocity = HeadInput.HeadVelocity(_heapAnchor.position, _parentAnchor.position, 0.2f, 1f);
            _parentAnchor.position += _velocity;
            _heapAnchor.position += _velocity;
        }
    }
}