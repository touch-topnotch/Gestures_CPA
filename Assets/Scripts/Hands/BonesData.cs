using Scripts.Static;
using UnityEngine;

namespace Scripts.Hands
{
    public class BonesData
    {
        public Vector3 rootPos;

        public Quaternion[] rotations;
        private HandType _type;

        public BonesData(HandType type)
        {
            _type = type;
        }
        public BonesData(in Transform[] points, in HandType type)
        {
            rootPos = points[0].position;
            rotations = Vector3Converter.convertToQuaternion(points);
            _type = type;
        }

        public HandType Type() => _type;
        
    }
}