using Scripts.Events;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using Zenject;

namespace Scripts.Movements
{
    public class XRMovement : Movement

    {
        [Range(0, 3f)] [SerializeField] protected float xzBoard;
        [Range(0, 3f)] [SerializeField] protected float yBoard;
        [Range(0, 10f)] [SerializeField] protected float jumpSpeed;
        [Range(0, 10f)] [SerializeField] protected float moveSpeed;
        
        [SerializeField]
        protected Transform pivot;

        public float XZBoard
        {
            get => xzBoard;
            set => xzBoard = math.clamp(value, 0, 5);
        }

        public float YBoard
        {
            get => yBoard;
            set => yBoard = math.clamp(value, 0, 5);
        }
        private Vector3 _velocity;
        
        public override void StartMove()
        {
            pivot.position = anchors.GetHead().position;
            base.StartMove();
        }

        protected override void UpdateVelocity()
        {
            _velocity =( HeadManipulations.HeadVelocity(pivot.position, anchors.GetHead().position, xzBoard, yBoard,
                moveSpeed,
                jumpSpeed) + Vector3.down * 5)/ 10;
            parentMoveController.Move(_velocity);
            
        }

    }
}