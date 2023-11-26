using Scripts.Static;
using UnityEngine;

namespace Scripts.Hands
{
    public class BonesData
    {
        public Vector3 rootPos;

        public Quaternion[] rotations;
        private readonly HandType _type;

        public BonesData(HandType type)
        {
            _type = type;
        }
        public BonesData(in Transform[] points, in HandType type)
        {
            _type = type;
            if (points == null)
                return;
            rootPos = points[0].position;
            rotations = VectorConverter.ToQuaternion(points);
            
        }

        public HandType Type() => _type;
        
    }
}