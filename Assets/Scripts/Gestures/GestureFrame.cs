using System;
using JetBrains.Annotations;
using Scripts.HandsLogic;
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

    public class HandsStruct
    {
        public HandUsedType HandUsed { get; private set; }

        private BonesData _left = new BonesData(HandType.left);
        private BonesData _right = new BonesData(HandType.right);

        public HandsStruct()
        {
            HandUsed = HandUsedType.NULL;
        }

        public HandsStruct(
            BonesData left,
            BonesData right
        )
        {
            LeftBones = left;
            RightBones = right;
        }

        public BonesData LeftBones
        {
            get => _left;
            set => _left = SetHandPoints(value, HandUsedType.LEFT);
        }

        public BonesData RightBones
        {
            get => _right;
            set => _right = SetHandPoints(value, HandUsedType.RIGHT);
        }

        private BonesData SetHandPoints(BonesData points, HandUsedType handUsed)
        {
            if (points != null && points.Exists())
                AddToEnum(handUsed);
            else
                RemoveFrEnum(handUsed);
            return points;
        }

        private void AddToEnum(HandUsedType hand)
        {
            HandUsed = hand switch
            {
                HandUsedType.LEFT => HandUsed is HandUsedType.LEFTNRIGHT or HandUsedType.RIGHT
                    ? HandUsedType.LEFTNRIGHT
                    : HandUsedType.LEFT,
                _ => HandUsed is HandUsedType.LEFTNRIGHT or HandUsedType.LEFT
                    ? HandUsedType.LEFTNRIGHT
                    : HandUsedType.RIGHT
            };
        }

        private void RemoveFrEnum(HandUsedType hand)
        {
            if (hand == HandUsedType.LEFTNRIGHT)
            {
                HandUsed = HandUsedType.NULL;
                return;
            }

            if (HandUsed == HandUsedType.LEFTNRIGHT)
            {
                HandUsed = hand == HandUsedType.LEFT ? HandUsedType.RIGHT : HandUsedType.LEFT;
                return;
            }

            if (HandUsed == hand)
            {
                HandUsed = HandUsedType.NULL;
            }
        }

        public delegate void HandManipulation<T>(T item, BonesData data);

        public static void SwitchManipulation<T>(HandsStruct target, HandManipulation<T> manipulate, T left, T right,
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
    }

    public class GestureFrame
    {
        public GestureFrame(string name, HandsStruct handsStruct)
        {
            Hands = handsStruct;
            this.name = name;
        }

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

        public HandsStruct Hands;

        public override string ToString()
        {
            return
                $"GestureFrame {name} has {Hands.HandUsed},\n leftBones = {Hands.LeftBones?.ToString()}, \n rightBones = {Hands.RightBones?.ToString()}";
        }
    }
}