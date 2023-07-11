using UnityEngine;

namespace CrossPlatform.Gestures
{
    public enum HandUsedE
    {
        NULL,
        LEFT,
        RIGHT,
        LEFTNRIGHT
    }
    public class GestureFrame
    
    { 
        [HideInInspector]
        public HandUsedE HandUsed = HandUsedE.NULL;
        
        public string Name;
        private Vector3[] _leftPoints;
        private Vector3[] _rightPoints;
        public Vector3[] LeftPoints
        {
            get => GetHandPoints(HandUsedE.LEFT, _leftPoints);
            set => SetHandPoints(HandUsedE.LEFT, value);
        }
        public Vector3[] RightPoints
        {
            get => GetHandPoints(HandUsedE.RIGHT, _leftPoints);
            set => SetHandPoints(HandUsedE.RIGHT, value);
        }
        private Vector3[] GetHandPoints(HandUsedE handUsed, Vector3[] points)
        {
            if (HandUsed == handUsed || HandUsed == HandUsedE.LEFTNRIGHT)
                return points;
            
            return null;
        }
        private void SetHandPoints(HandUsedE handUsed, Vector3[] points)
        {
            if (points == null)
            {
                RemoveFrEnum(handUsed);
                _leftPoints = null;
            }
            else
            {
                AddToEnum(handUsed);
                _leftPoints = points;
            }
        }
        private void AddToEnum(HandUsedE hand)
        {
            if (hand == HandUsedE.LEFT)
                HandUsed = (HandUsed == HandUsedE.LEFTNRIGHT || HandUsed == HandUsedE.RIGHT)
                    ? HandUsedE.LEFTNRIGHT
                    : HandUsedE.LEFT;
            else
                HandUsed = (HandUsed == HandUsedE.LEFTNRIGHT || HandUsed == HandUsedE.LEFT)
                    ? HandUsedE.LEFTNRIGHT
                    : HandUsedE.RIGHT;
        }
        private void RemoveFrEnum(HandUsedE hand)
        {
            if (hand == HandUsedE.LEFTNRIGHT)
            {
                HandUsed = HandUsedE.NULL;
                return;
            }

            if (HandUsed == HandUsedE.LEFTNRIGHT)
            {
                if (hand == HandUsedE.LEFT)
                    HandUsed = HandUsedE.RIGHT;
                else
                    HandUsed = HandUsedE.LEFT;
                return;
            }

            if (HandUsed == hand)
            {
                HandUsed = HandUsedE.NULL;
            }
        }
    }

}

