using Scripts.Movements;
using Scripts.Static;
using UnityEngine;
using UnityEngine.UIElements;

namespace Scripts.PlayerLogic
{
    public class XRRig :Rig
    {
        [SerializeField] private Transform cameraTarget;
        
        [SerializeField] private XRMovement _movement;
        [SerializeField] private Vector3 _centerOffset;
        public override bool isMoved() => _movement.isMoved();

        public override void StartMove() => _movement.StartMove();

        public override void StopMove() => _movement.StopMove();

        protected override void Start()
        {
            base.Start();
            Centrize();
        }
        private void Update()
        {
            // Update body rotation
            var centrisedPosition = cameraTarget.localPosition + _centerOffset;
            var eulerAngles = cameraTarget.localEulerAngles;
            anchors.Head.localPosition = new Vector3(0, centrisedPosition.y, 0);
            anchors.Head.localRotation = Quaternion.Euler(eulerAngles.x, 0, eulerAngles.z);
            anchors.Body.localRotation =  Quaternion.Euler(0, eulerAngles.y, 0);
            if (!isMoved())
            {
                anchors.Body.localPosition = new Vector3(centrisedPosition.x, 0, centrisedPosition.z);
            }
            else
            {
                anchors.Head.localPosition = Calculations.Rotate(centrisedPosition, eulerAngles.y);
            }
        }
        private static Vector3 ClampRotation(Vector3 rotation)
        {
            // rotation.x = rotation.x > 180 ? rotation.x - 360 : rotation.x;
            //rotation.z = rotation.z > 180 ? rotation.z - 360 : rotation.z;
            return rotation;
        }
        protected override void Centrize()
        {
            var position = cameraTarget.localPosition;
            _centerOffset = new Vector3(-position.x, 0, -position.z);
        }

    }
 
}