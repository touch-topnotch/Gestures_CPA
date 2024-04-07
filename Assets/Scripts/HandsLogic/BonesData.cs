using System;
using Scripts.Static;
using Scripts.Systems;
using UnityEngine;

namespace Scripts.HandsLogic
{
    public class BonesData
    {
        public Vector3 rootPos;

        public Quaternion[] rotations = Array.Empty<Quaternion>();
        public bool Exists() => rotations != null && rotations.Length != 0;
        private readonly HandType _type;
        private Vector3 nullVector = Vector3.zero;
        public BonesData(HandType type)
        {
            _type = type;
        }
        public BonesData(in HandType type, in Quaternion[] rotations = null,Vector3 rootPos = default)
        {
            _type = type;
            if (this.rotations == null)
                return;
            this.rootPos = rootPos;
            this.rotations = rotations;
            
        }
        public BonesData(in Transform[] points, in HandType type)
        {
            _type = type;
            if (points == null || points.Length == 0)
                return;
            rootPos = points[0].localPosition;
            rotations = new Quaternion[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                rotations[i] = points[i].localRotation;
            }
        }
        public HandType Type() => _type;

        public override string ToString()
        {
            return $"Root: {rootPos}, rotations: {Debugger.arrayToString<Quaternion>(rotations)}";
        }
    }
}