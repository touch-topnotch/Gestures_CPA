using System.Collections.Generic;
using UnityEngine;

namespace CrossPlatform.Gestures
{
    public class GesturesLibrary: MonoBehaviour
    {
        public List<GestureFrame> GestureFrames { get; private set; } = new List<GestureFrame>();
        public List<DynamicGesture> DynamicGestures { get; private set; } = new List<DynamicGesture>();
        public void SetGestureFrame(GestureFrame frame)
        {
            GestureFrames.Add(frame);
            
            if (frame.Name != frame.BaseName)
            {
                if (DynamicGestures.Count == 0)
                {
                    DynamicGestures.Add(new DynamicGesture(frame.BaseName));
                    DynamicGestures[0].AddFrame(frame);
                    return;
                }

                bool f = false;
                foreach (DynamicGesture dynamicGesture in DynamicGestures)
                {
                    if (dynamicGesture.Name == frame.BaseName)
                    {
                        dynamicGesture.AddFrame(frame);
                        return;
                    }
                }

                if (!f)
                {
                    DynamicGestures.Add(new DynamicGesture(frame.BaseName));
                    DynamicGestures[0].AddFrame(frame);
                }
            }
        }
        public DynamicGesture GetDynamicGesture(string name)
        {
            foreach (var frame in DynamicGestures)
            {
                if(frame.Name == name)
                    return frame;
            }
            throw new System.Exception("No gesture with this name");
        }
        
        public GestureFrame GetGestureFrame(string name)
        {
            foreach (var frame in GestureFrames)
            {
                if(frame.Name == name)
                    return frame;
            }
            throw new System.Exception("No gesture with this name");
        }
    }
}