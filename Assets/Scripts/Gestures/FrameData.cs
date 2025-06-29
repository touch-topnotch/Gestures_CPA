using System;
using JetBrains.Annotations;
using Scripts.Gesture_Editor_SDK.Realtime;
using Scripts.HandsLogic;
using Scripts.PlayerLogic;
using UnityEngine;
using UnityEngine.XR;

namespace Scripts.Gestures
{
    public enum HandUsedType
    {
        LEFT,
        RIGHT,
        LEFTNRIGHT,
        NULL
    }

    public class FrameData
    {
        private string _name;
        public string baseName { get; private set; }

        public string name
        {
            get => _name;
            set
            {
                _name = value;
                baseName = value.Split('_')?[0];
            }
        }

        public HandUsedType HandUsed { get; private set; }

        private BonesData _left = new BonesData(HandType.left);
        private BonesData _right = new BonesData(HandType.right);

        public FrameData(string name)
        {
            this.name = name;
            HandUsed = HandUsedType.NULL;
        }

        public FrameData(
            string name,
            BonesData left,
            BonesData right
        )
        {
            this.name = name;
            LeftBones = left;
            RightBones = right;
        }

        public FrameData(FrameData previous)
        {
            this.name = previous.name;
            LeftBones = isNullOrEmpty(previous.LeftBones) ? null : new BonesData(HandType.left, previous.LeftBones);
            RightBones = isNullOrEmpty(previous.RightBones) ? null : new BonesData(HandType.right, previous.RightBones);
        }

        public FrameData(FrameData previous, Transform parent)
        {
            this.name = previous.name;
            LeftBones = isNullOrEmpty(previous.LeftBones) ? null : new BonesData(HandType.left, previous.LeftBones);
            RightBones = isNullOrEmpty(previous.RightBones) ? null : new BonesData(HandType.right, previous.RightBones);
            LeftBones?.SetParent(parent);
            RightBones?.SetParent(parent);
        }


        public BonesData LeftBones
        {
            get => _left;
            set
            {
                _left = value;
                HandUsed = RefreshType();
            }
        }

        public BonesData RightBones
        {
            get => _right;
            set
            {
                _right = value;
                HandUsed = RefreshType();
            }
        }

        public bool isNullOrEmpty(BonesData data) => data == null || !data.Exists();

        private HandUsedType RefreshType()
        {
            var left = _left != null && _left.Exists();
            var right = _right != null && _right.Exists();

            if (left && right)
                return HandUsedType.LEFTNRIGHT;
            if (right)
                return HandUsedType.RIGHT;
            if (left)
                return HandUsedType.LEFT;
            return HandUsedType.NULL;
        }

        public delegate void HandManipulation<T>(T item, BonesData data);

        public static void SwitchManipulation<T>(FrameData target, HandManipulation<T> manipulate, T left, T right,
            Action nullCallback = null)
        {
            switch (target.HandUsed)
            {
                case HandUsedType.NULL:
                    if (nullCallback != null) nullCallback();
                    return;
                case HandUsedType.LEFT:
                    manipulate(left, target.LeftBones);
                    return;
                case HandUsedType.RIGHT:
                    manipulate(right, target.RightBones);
                    return;
                case HandUsedType.LEFTNRIGHT:
                    manipulate(left, target.LeftBones);
                    manipulate(right, target.RightBones);
                    return;
            }
        }

        public void SwitchManipulation<T>(HandManipulation<T> manipulate, T left, T right,
            Action nullCallback = null) =>
            SwitchManipulation(this, manipulate, left, right, nullCallback);

        public override string ToString()
        {
            return
                $"FrameData {name} has {HandUsed},\n leftBones = {LeftBones?.ToString()}, \n rightBones = {RightBones?.ToString()}";
        }

        public FrameData ParentedFrame(Transform parent)
        {
            return new FrameData(this, parent);
        }

        public FrameData AttachedToPlayer()
        {
            return ParentedFrame(PlayerData.local.bodyAnchors.Body);
        }
    }
}