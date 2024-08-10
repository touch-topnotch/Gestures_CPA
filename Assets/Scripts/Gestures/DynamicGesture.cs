using System.Collections.Generic;
using Scripts.Gestures.GGUI;
using Scripts.PlayerLogic;
using UnityEngine;

namespace Scripts.Gestures
{

    
    public class DynamicGesture
    {
    
        public readonly string Name;
        public FrameDetected OnFrameDetected;
        public List<GestureFrame> Frames = new List<GestureFrame>();
        public GUIGesture Graphics;
        private int _currentGesture = 0;
        public DynamicGesture(string name)
        {
            Name = name;
        }
        
        public void AddFrame(GestureFrame frame)
        {
            Frames.Add(frame);
        }


        public GestureFrame GetGestureFrame() => _currentGesture < Frames.Count ? Frames[_currentGesture] : null;
        public void FrameRecognized()
        {
            Debug.Log($"{GetGestureFrame().name} recognized!");
            OnFrameDetected?.Invoke(_currentGesture, GetGestureFrame());
            NextFrame();
        }

        public void NextFrame()
        {
            if (_currentGesture >= Frames.Count)
            {
               // AllFramesDetected();
                return;
            }

            _currentGesture += 1;
        }

        //private void AllFramesDetected() => _currentGesture = 0;

    }
}

