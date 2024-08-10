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
        public HandUsedType HandUsed = HandUsedType.NULL;
        
        private Vector3[] _leftPoints;
        private Vector3[] _rightPoints;
        public Vector3[] LeftPoints
        {
            get => GetHandPoints(HandUsedType.LEFT, _leftPoints);
            set => _leftPoints = SetHandPoints(HandUsedType.LEFT, value);
        }
        public Vector3[] RightPoints
        {
            get => GetHandPoints(HandUsedType.RIGHT, _rightPoints);
            set => _rightPoints = SetHandPoints(HandUsedType.RIGHT, value);
        }
        private Vector3[] GetHandPoints(HandUsedType handUsed, Vector3[] points)
        {
            if (HandUsed == handUsed || HandUsed == HandUsedType.LEFTNRIGHT)
                return points;
            
            return null;
        }
        private Vector3[] SetHandPoints(HandUsedType handUsed, Vector3[] points)
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
            if (hand == HandUsedType.LEFT)
                HandUsed = (HandUsed == HandUsedType.LEFTNRIGHT || HandUsed == HandUsedType.RIGHT)
                    ? HandUsedType.LEFTNRIGHT
                    : HandUsedType.LEFT;
            else
                HandUsed = (HandUsed == HandUsedType.LEFTNRIGHT || HandUsed == HandUsedType.LEFT)
                    ? HandUsedType.LEFTNRIGHT
                    : HandUsedType.RIGHT;
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
                if (hand == HandUsedType.LEFT)
                    HandUsed = HandUsedType.RIGHT;
                else
                    HandUsed = HandUsedType.LEFT;
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

