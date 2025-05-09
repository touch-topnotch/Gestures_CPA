using Scripts.HandsLogic;
using Scripts.Movements;
using Scripts.Static;
using Scripts.Systems;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.UIElements;
using UnityEngine.XR.Hands.Samples.VisualizerSample;

namespace Scripts.PlayerLogic
{
    public class XRRig : Rig
    {
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private XRMovement _movement;
        private Vector3 _centerOffset;
        public override bool isMoved() => _movement.isMoved();

        public override void StartMove() => _movement.StartMove();

        public override void StopMove() => _movement.StopMove();

#if UNITY_EDITOR
        [Button("Add Missing Components")]
        public override void AddMissingComponents()
        {
            base.AddMissingComponents();
            var rig = Selection.activeGameObject.GetComponentInChildren<XRRig>();
            _cameraTarget ??= rig.GetComponentInChildren<TrackedPoseDriver>().transform;
            _movement ??= rig.GetComponentInChildren<XRMovement>();
            _movement.AddMissingComponents();
            Selection.activeGameObject.GetComponentInChildren<CustomHandVisualizer>().AddMissingComponents();
        }
#endif
        public override void Initialize(PlayerData data)
        {
            base.Initialize(data);
            headInteraction.onHeadInteraction += (e) =>
            {
                if (e == HeadInteractionType.Shaking)
                {
                    if (_movement.isMoved())
                        _movement.StopMove();
                    else
                    {
                        Centrize();
                        _movement.StartMove();
                    }
                }
            };
        }

        private void Update()
        {
            // Update body rotation
            var centrisedPosition = _cameraTarget.localPosition + _centerOffset;
            // Debug.Log(centrisedPosition);
            var eulerAngles = _cameraTarget.localEulerAngles;

            anchors.Head.localPosition = new Vector3(0, centrisedPosition.y, 0);
            anchors.Head.localRotation = Quaternion.Euler(eulerAngles.x, 0, eulerAngles.z);

            anchors.Body.localPosition = new Vector3(centrisedPosition.x, 0, centrisedPosition.z);
            anchors.Body.localRotation = Quaternion.Euler(0, eulerAngles.y, 0);
        }

        private static Vector3 ClampRotation(Vector3 rotation)
        {
            // rotation.x = rotation.x > 180 ? rotation.x - 360 : rotation.x;
            //rotation.z = rotation.z > 180 ? rotation.z - 360 : rotation.z;
            return rotation;
        }

        protected override void Centrize()
        {
            var position = _cameraTarget.localPosition;
            _centerOffset = new Vector3(-position.x, 0, -position.z);
            hands.transform.localPosition = _centerOffset;
            Update();
            _movement.Centrize();
        }
        // мы двигаем голову, нужно двигать все, кроме тела
    }
}