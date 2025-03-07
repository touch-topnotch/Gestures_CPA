using Scripts.Movements;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    public class XRRig :Rig
    {
        [SerializeField] private Transform cameraTarget;
        
        [SerializeField] private XRMovement _movement;
     
        public override bool isMoved() => _movement.isMoved();

        public override void StartMove() => _movement.StartMove();

        public override void StopMove() => _movement.StopMove();

        private void Update()
        {
        
            // Update body rotation
            var position = cameraTarget.position;
            var eulerAngles = cameraTarget.eulerAngles;
            anchors.Head.localPosition = new Vector3(0, position.y, 0);
            anchors.Head.localRotation = Quaternion.Euler(eulerAngles.x, 0, eulerAngles.z);
            anchors.Body.localRotation =  Quaternion.Euler(0, eulerAngles.y, 0);
            if (!isMoved())
            {
                anchors.Body.localPosition = new Vector3(position.x, 0, position.z);
                // Vector3 localPosition = anchors.Head.localPosition;
                // var position = anchors.Body.localPosition;
                // position = Vector3.Lerp(position,
                //     new Vector3(localPosition.x, localPosition.y - 1.6f, localPosition.z), Time.deltaTime*bodyPositionSpeed);
                // anchors.Body.localPosition = position;
            }
            
            // Quaternion.Lerp( anchors.Body.localRotation,
            //     Quaternion.Euler(new Vector3(0, anchors.Head.localEulerAngles.y, 0)),
            //     Time.deltaTime*bodyRotationSpeed );
        }
        private static Vector3 ClampRotation(Vector3 rotation)
        {
            // rotation.x = rotation.x > 180 ? rotation.x - 360 : rotation.x;
            //rotation.z = rotation.z > 180 ? rotation.z - 360 : rotation.z;
            return rotation;
        }

    }
}