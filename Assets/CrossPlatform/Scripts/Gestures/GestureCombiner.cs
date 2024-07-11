

using System.Collections.Generic;

namespace CrossPlatform.Gestures
{
    public class GestureCombiner
    {
        private IGestureRecognizer _recognizer;
        private Dictionary<string, DynamicGesture> _allGestures;
        private string path;
        

        GestureCombiner(IGestureRecognizer recognizer)
        {
            _recognizer = recognizer;
        }
       
        private void CreateCombination()
        {
            
        }
    }
}