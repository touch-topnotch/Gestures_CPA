using JetBrains.Annotations;
using Scripts.Hands;
using UnityEngine;

namespace Scripts.Gestures
{
    public enum HandUsedType
    {
        NULL,
        LEFT,
        RIGHT,
        LEFTNRIGHT
    }

    public class HandsStruct
    {
        public HandUsedType HandUsed { get; private set; } = HandUsedType.NULL;
        
        private BonesData _left;
        private BonesData _right;

        public HandsStruct(
            Transform[] leftPoints = null,
            Transform[] rightPoints = null
        )
        {
            LeftBones = new BonesData(leftPoints, HandType.left);
            RightBones = new BonesData(rightPoints, HandType.right);
        }
        public BonesData LeftBones
        {
            get => GetHandPoints(_left, HandUsedType.LEFT);
            set => _left = SetHandPoints(value, HandUsedType.LEFT);
        }
        public BonesData RightBones
        {
            
            get => GetHandPoints(_right, HandUsedType.RIGHT);
            set => _right = SetHandPoints(value, HandUsedType.RIGHT);
        }
        private BonesData GetHandPoints(BonesData data, HandUsedType handUsed)
        {
            if (HandUsed == HandUsedType.NULL)
                return null;
            return data;
        }
        private BonesData SetHandPoints(BonesData points, HandUsedType handUsed)
        {
            if (points == null)
            {
                RemoveFrEnum(handUsed);
            }
            else
            {
                AddToEnum(handUsed);
            }

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
    }
    public class GestureFrame
    {
        private string _name;
        public string baseName { get; private set; }

        public string name
        {
            get => _name;
            set
            {
                _name = value;
                baseName = value.Split('_')[0];
            }
        }

        public HandsStruct Hands = new();
    }
}

