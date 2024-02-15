using Scripts.Movements;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    public class XRRig :Rig
    {
        [SerializeField] protected Transform  _trackedPoseDriverTransform;
        
        [SerializeField] private XRMovement _movement;
        [Range(0, 10)] [SerializeField] private float bodyPositionSpeed;
        [Range(0, 10)] [SerializeField] private float headPositionSpeed;
        [Range(0, 1000)] [SerializeField] private float bodyRotationSpeed;
     
        [Range(0, 1000)] [SerializeField] private float headRotationSpeed;
        public override bool isMoved() => _movement.isMoved();

        public override void StartMove() => _movement.StartMove();

        public override void StopMove() => _movement.StopMove();

        private void Update()
        {
   
            // Get the tracked pose driver's localEulerAngles once
            Vector3 localEulerAngles = ClampRotation(_trackedPoseDriverTransform.localEulerAngles);
           
            // Update body rotation
            anchors.Body.localRotation = Quaternion.Lerp( anchors.Body.localRotation,Quaternion.Euler(new Vector3(0, localEulerAngles.y, 0)),Time.deltaTime*bodyRotationSpeed );

            // Update head rotation
            anchors.Head.localRotation = Quaternion.Lerp(anchors.Head.localRotation,
                Quaternion.Euler(new Vector3(localEulerAngles.x, 0, localEulerAngles.z)),
                    Time.deltaTime * headRotationSpeed);

            if (!isMoved())
            {
                Vector3 localPosition = _trackedPoseDriverTransform.localPosition;
                anchors.Body.localPosition = Vector3.Lerp(anchors.Body.localPosition,
                    new Vector3(localPosition.x, anchors.Body.localPosition.y, localPosition.z), bodyPositionSpeed);

                anchors.Head.localPosition = Vector3.Lerp(anchors.Head.localPosition,
                    new Vector3(0,1.8f, 0), headPositionSpeed);
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