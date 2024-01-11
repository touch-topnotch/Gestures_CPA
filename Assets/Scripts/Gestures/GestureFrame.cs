using JetBrains.Annotations;
using Scripts.Hands;
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
            if (points.Exists())
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
                baseName = value.Split('_')[0];
            }
        }

        public HandsStruct Hands;
    }
}

