using Scripts.Hands;

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
        
        private BonesData _left;
        private BonesData _right;
        public BonesData LeftBones
        {
            
            
            get => GetHandPoints(HandUsedType.LEFT, _left);
            set => _left = SetHandPoints(HandUsedType.LEFT, value);
        }
        public BonesData RightBones
        {
            
            get => GetHandPoints(HandUsedType.RIGHT, _right);
            set => _right = SetHandPoints(HandUsedType.RIGHT, value);
        }
        private BonesData GetHandPoints(HandUsedType handUsed, BonesData data)
        {
            if (HandUsed == HandUsedType.NULL)
                return null;
            return data;
        }
        private BonesData SetHandPoints(HandUsedType handUsed, BonesData points)
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

