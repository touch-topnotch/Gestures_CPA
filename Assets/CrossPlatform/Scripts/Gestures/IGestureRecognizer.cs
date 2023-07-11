using System.Collections.Generic;

namespace CrossPlatform.Gestures
{
    public interface IGestureRecognizer
    {
        void Recognize(int[] handPoints, List<GestureFrame> currentGestures);
    }
}