using System;
using JetBrains.Annotations;
using Scripts.PlayerLogic;
using Scripts.Static;
using Scripts.Systems;
using UnityEngine;

namespace Scripts.HandsLogic
{
    public class BonesData
    {
        private Vector3 fixedRootPos;
        private Quaternion fixedRootRot;
        
        public Vector3 rootPos;
        public Quaternion[] rotations = Array.Empty<Quaternion>();
        public bool Exists() => rotations != null && rotations.Length != 0;
        public readonly HandType type;
        private Vector3 nullVector = Vector3.zero;
        public BonesData(HandType type)
        {
            this.type = type;
        }
        public BonesData(in HandType type, in Quaternion[] rotations, Vector3 rootPos = default)
        {
            this.type = type;
            this.rootPos = rootPos;
            this.fixedRootPos = rootPos;
            this.rotations = rotations;
            if(rotations != null)
                fixedRootRot = rotations[0];

        }
        public BonesData(in Transform[] points, in HandType type)
        {
        
            var rot = new Quaternion[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                rot[i] = points[i].localRotation;
            }
            this.type = type;
            rootPos = points[0].localPosition;
            fixedRootPos = rootPos;
            rotations = rot;
            if (rotations != null)
                fixedRootRot = rot[0];
        }

        public override string ToString()
        {
            return $"Root: {rootPos}, rotations: {Debugger.arrayToString<Quaternion>(rotations)}";
        }
      
        public void ListenAnchors(in BodyAnchors anchors)
        {
            rootPos  = fixedRootPos +  anchors.Root.position;
            rootPos = anchors.Body.TransformPoint(rootPos);
            // rotation of object is a rotation of parent * rotation of object
            rotations[0] = anchors.Body.rotation * this.fixedRootRot;
        }
    }
}