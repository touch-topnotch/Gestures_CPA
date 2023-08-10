using Scripts.Events;
using UnityEngine;
using Zenject;

namespace Scripts.Movements
{
    public class XRMovement : Movement

    {
        [SerializeField] protected Transform headAnchor;
        [Range(0, 3f)] [SerializeField] protected float xzBoard;
        [Range(0, 3f)] [SerializeField] protected float yBoard;
        [Range(0, 10f)] [SerializeField] protected float jumpSpeed;
        [Range(0, 10f)] [SerializeField] protected float moveSpeed;
        
        [SerializeField]
        protected Transform pivot;
        
        private Vector3 _velocity;
        
        public override void StartMove()
        {
            pivot.position = headAnchor.position;
            base.StartMove();
        }

        protected override void UpdateVelocity()
        {
            _velocity = HeadManipulations.HeadVelocity(pivot.position, headAnchor.position, xzBoard, yBoard,moveSpeed,
                jumpSpeed);
            ParentRigidbody.AddForce(_velocity);
        }

    }
}