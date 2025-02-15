using Scripts.Movements;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    public class XRRig :Rig
    {
        [SerializeField] protected Transform  _trackedPoseDriverTransform;
        
        [SerializeField] private XRMovement _movement;
        public override bool isMoved() => _movement.isMoved();

        public override void StartMove() => _movement.StartMove();

        public override void StopMove() => _movement.StopMove();

        private void Update()
        {
            // Get the tracked pose driver's localEulerAngles once
            Vector3 localEulerAngles = ClampRotation(_trackedPoseDriverTransform.localEulerAngles);

            // Update body rotation
            anchors.Body.localRotation = Quaternion.Euler(new Vector3(0, localEulerAngles.y, 0));

            // Update head rotation
            anchors.Head.localRotation = Quaternion.Euler(new Vector3(localEulerAngles.x, 0, localEulerAngles.z));
            
            if (!isMoved())
            {
                Vector3 localPosition = _trackedPoseDriverTransform.localPosition;
                anchors.Body.localPosition = new Vector3(localPosition.x, anchors.Body.localPosition.y, localPosition.z);
                anchors.Head.localPosition = new Vector3(0, localPosition.y, 0);
            }
        }
        private static Vector3 ClampRotation(Vector3 rotation)
        {
            // rotation.x = rotation.x > 180 ? rotation.x - 360 : rotation.x;
            //rotation.z = rotation.z > 180 ? rotation.z - 360 : rotation.z;
            return rotation;
        }

    }
}