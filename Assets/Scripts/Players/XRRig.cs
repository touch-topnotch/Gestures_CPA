using Scripts.Movements;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    public class XRRig :Rig
    {
        [SerializeField] private XRMovement _movement;
        
        [Range(0, 10)] [SerializeField] private float bodyPositionSpeed;
        [Range(0, 10)] [SerializeField] private float bodyRotationSpeed;
     
        public override bool isMoved() => _movement.isMoved();

        public override void StartMove() => _movement.StartMove();

        public override void StopMove() => _movement.StopMove();

        private void Update()
        {

            // Update body rotation
            anchors.Body.localRotation = 
                Quaternion.Lerp( anchors.Body.localRotation,
                    Quaternion.Euler(new Vector3(0, anchors.Head.localEulerAngles.y, 0)),
                    Time.deltaTime*bodyRotationSpeed );

            if (!isMoved())
            {
                Vector3 localPosition = anchors.Head.localPosition;
                var position = anchors.Body.localPosition;
                position = Vector3.Lerp(position,
                    new Vector3(localPosition.x, localPosition.y - 1.6f, localPosition.z), Time.deltaTime*bodyPositionSpeed);
                anchors.Body.localPosition = position;
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