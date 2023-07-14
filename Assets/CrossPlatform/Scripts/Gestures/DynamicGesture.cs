
using System.Collections.Generic;

namespace  CrossPlatform.Gestures
{
    public class DynamicGesture
    {
        public string Name;
        public List<GestureFrame> Frames = new List<GestureFrame>();

        public DynamicGesture(string name)
        {
            Name = name;
        }
        public void AddFrame(GestureFrame frame)
        {
            Frames.Add(frame);
        }
        
        
    }
}

