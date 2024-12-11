using System.Collections.Generic;
using Scripts.Events;
using Scripts.Gestures.GGUI;
using Scripts.Hands;
using UnityEngine;

namespace Scripts.Gestures
{

    
    public class DynamicGesture
    {
    
        public readonly string Name;
        public readonly FrameDetected onFrameDetected = new();
        public List<GestureFrame> Frames = new();
        public GUIGesture graphics;
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
            onFrameDetected?.Invoke(_currentGesture, GetGestureFrame());
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

        public void AllFramesDetected() => _currentGesture = 0;

        public void LogFrames()
        {
            var log = $"Gesture  {Name} contains: ";
            for (int i = 0; i < Frames.Count; i++)
            {
                log += Frames[i].name + " - base name: "+ Frames[i].baseName + ", ";
            }

            Debug.Log(log);
        }

        public void AddGraphics(PlayerHands hands)
        {
            graphics.Construct(hands);
            onFrameDetected.AddListener(graphics.ShowEffects);
        }
    }
}