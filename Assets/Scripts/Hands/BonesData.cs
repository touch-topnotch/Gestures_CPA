using Scripts.Static;
using UnityEngine;

namespace Scripts.Hands
{
    public class BonesData
    {
        public Vector3 RootPos;

        private Quaternion[] _rotations;
        private HandType _type;
        public BonesData(in Transform[] points, in HandType type)
        {
            RootPos = points[0].position;
            Rotations = Vector3Converter.convertToQuaternion(points);
            _type = type;
        }

        public HandType Type() => _type;
        
        public Quaternion[] Rotations
        {
            get => _rotations;
            set => _rotations = value;
        }
    }
}