using Scripts.Static;
using UnityEngine;

namespace Scripts.Hands
{
    public class BonesData
    {
        private Vector3[] _positions;
        private Quaternion[] _rotations;
        private HandType _type;
        public BonesData(in Transform[] transf, in HandType type)
        {
            Positions = Vector3Converter.TransfToPos(transf);
            Rotations = Vector3Converter.TransfToRot(transf);
            _type = type;
        }

        public HandType Type() => _type;
        public Vector3[] Positions
        {
            get => _positions;
            set => _positions = value;
        }
        public Quaternion[] Rotations
        {
            get => _rotations;
            set => _rotations = value;
        }
    }
}