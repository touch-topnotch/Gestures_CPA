using Scripts.Systems;
using UnityEngine;

namespace Scripts.HandsLogic
{
    public class BonesData
    {
        public Vector3 rootPos => _isListened ? TransformRoot() : _fixedRootPos;
        public Quaternion[] rotations => _isListened ? TransformRotations() : _rotations;
        public bool Exists() => rotations != null && rotations.Length != 0;

        public readonly HandType type;

        private readonly Vector3 _fixedRootPos;
        private readonly Quaternion _fixedRootRot;
        private Quaternion[] _rotations;
        private bool _isListened;
        private Transform _parent;

        public BonesData(HandType type)
        {
            this.type = type;
        }

        public BonesData(HandType type, BonesData data)
        {
            this.type = type;
            if (data == null)
                return;
            _fixedRootPos = data._fixedRootPos;
            _fixedRootRot = data._fixedRootRot;
            _rotations = data._rotations;
        }

        public BonesData(in HandType type, in Quaternion[] rotations, Vector3 rootPos = default)
        {
            this.type = type;
            _fixedRootPos = rootPos;
            _rotations = rotations;
            if (rotations != null)
                _fixedRootRot = rotations[0];
        }

        public BonesData(in Transform[] points, in HandType type)
        {
            var rot = new Quaternion[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                rot[i] = points[i].localRotation;
            }

            this.type = type;
            _fixedRootPos = points[0].localPosition;
            _rotations = rot;
            if (rotations != null)
                _fixedRootRot = rot[0];
        }

        public override string ToString()
        {
            return $"Root: {rootPos}, rotations: {Debugger.arrayToString<Quaternion>(rotations)}";
        }

        public void SetParent(Transform transform) // Listen при любом вызове
        {
            _parent = transform;
            if (!Exists())
            {
                return;
            }

            _isListened = true;
        }

        public void RemoveParent()
        {
            _isListened = false;
        }

        private Vector3 TransformRoot() => _parent.TransformVector(_fixedRootPos);

        private ref Quaternion[] TransformRotations()
        {
            _rotations[0] = _parent.rotation * _fixedRootRot;
            return ref _rotations;
        }
    }
}