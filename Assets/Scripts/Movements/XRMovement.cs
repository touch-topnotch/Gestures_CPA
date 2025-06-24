using Scripts.PlayerLogic;
using Unity.Mathematics;
using UnityEngine;

namespace Scripts.Movements
{
    public class XRMovement : RigComponent
    {
        public bool moveOnAwake;
        [SerializeField] protected CharacterController parentMoveController;
        [SerializeField] protected float gravity;

        [Range(0, 3f)] [SerializeField] protected float xzBoard;
        [Range(0, 3f)] [SerializeField] protected float yBoard;
        [Range(0, 1f)] [SerializeField] protected float jumpSpeed;
        [Range(0, 1f)] [SerializeField] protected float moveSpeed;
        [SerializeField] protected Vector2 velocityBoard;

        [SerializeField] protected Transform xrCameraCenter;

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

        [SerializeField] private Vector3 _velocity;

        [SerializeField] private bool _isMoved;
        public bool isMoved() => _isMoved;

        private void Start()
        {
            if (moveOnAwake)
            {
                StartMove();
            }
        }

        public void Centrize()
        {
            xrCameraCenter.position = inherited.anchors.Head.position;
        }

        public void StartMove()
        {
            _isMoved = true;
            Centrize();
            Debug.Log("Movement started");
        }

        public void StopMove()
        {
            _isMoved = false;
            Debug.Log("Movement stopped");
        }

        protected void Update()
        {
            if (!_isMoved)
                return;
            _velocity = (HeadManipulations.HeadVelocity(xrCameraCenter.position, inherited.anchors.Head.position,
                XZBoard, YBoard,
                moveSpeed,
                jumpSpeed) + Vector3.down * gravity) / 10;

            parentMoveController.Move(ClampVelocity(_velocity));
        }

        private Vector3 ClampVelocity(Vector3 velocity)
        {
            velocity.x = Mathf.Clamp(velocity.x, -velocityBoard.x, velocityBoard.x);
            velocity.y = Mathf.Clamp(velocity.y, -velocityBoard.y, velocityBoard.y);
            velocity.z = Mathf.Clamp(velocity.z, -velocityBoard.x, velocityBoard.x);
            return velocity;
        }

        protected override bool shouldAddMissingComponents =>
            !(parentMoveController && xrCameraCenter);

        public override void AddMissingComponents()
        {
            xrCameraCenter ??= inherited.anchors.Root.Find("XR_Camera_Center");
            parentMoveController ??= inherited.GetComponentInChildren<CharacterController>();
        }
    }
}