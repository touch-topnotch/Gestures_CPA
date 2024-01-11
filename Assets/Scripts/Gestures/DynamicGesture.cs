using System.Collections.Generic;
using Scripts.Events;
using Scripts.Gestures.Classes;
using Scripts.Gestures.GGUI;
using Scripts.Hands;
using Scripts.Static;
using UnityEngine;

namespace Scripts.Gestures
{

    public enum GestureType
    {
        CONTROL,
        ABILITY,
        HIT,
        ULTIMATE,
    }
    public class DynamicGesture
    {
        public List<GestureFrame> frames{ get;}
        public GestureType gestureType { get; }
        private readonly FrameDetected onFrameDetected = new();
        
        
        private GUIGesture _gui;
        private GestureCall _gestureCall;
        
       
        private string _name;
        private int _currentGesture = 0;
        public string Name
        {
            get => _name;
            private set
            {
                // if value is null, then set name to random string of 5 characters
                if (value == null || value.Length < 3f)
                {
                    Debug.LogWarning("Name of gesture " + value + " is too short!");
                    _name = Calculations.RandomString(5, value);
                }
                _name = value ?? Calculations.RandomString(5);
            }
        }
        public DynamicGesture(string name, List<GestureFrame> frames, GestureType gestureType, GUIGesture guiGesture, GestureCall gestureCall)
        {
            Name = name;
            
            this.gestureType = gestureType;
            this.frames = frames;
            
            _gui = guiGesture;
            _gestureCall = gestureCall;
        }

        public DynamicGesture(string name)
        {
            Name = name;
            
            frames = new();
            gestureType = GestureType.HIT;

            _gui = GestureFactory.SetDynamicGesture(name);
            _gestureCall = new Melee();
        }
        
        public void AddFrame(GestureFrame frame)
        {
            frames.Add(frame);
        }
        
        public GestureFrame GetGestureFrame() => _currentGesture < frames.Count ? frames[_currentGesture] : null;
        
        public GestureFrame GetNextFrameOf(GestureFrame frame)
        {
            if (frame == null)
                return frames[0];
            
            var index = frames.IndexOf(frame);
            if (index == -1)
                return null;
            
            return index + 1 < frames.Count ? frames[index + 1] : null;
        }
        public void FrameRecognized()
        {
            Debug.Log($"Frame {GetGestureFrame().name} recognized!");
            onFrameDetected?.Invoke(_currentGesture, GetGestureFrame());
            NextFrame();
        }

        public void NextFrame()
        {
            if (_currentGesture >= frames.Count)
            {
                // AllFramesDetected();
                return;
            }

            _currentGesture += 1;
        }

        public void AllFramesDetected()
        {
            _currentGesture = 0;
            _gestureCall.Start();
        }

        public void LogFrames()
        {
            var log = $"Gesture {Name} contains: ";
            for (int i = 0; i < frames.Count; i++)
            {
                log += frames[i].name + " - base name: "+ frames[i].baseName + ", ";
            }

            Debug.Log(log);
        }

        public void AddGraphicsToRigHands(PlayerHands hands)
        {
            if (_gui == null)
                return;
            _gui.Construct(hands);
            onFrameDetected.AddListener(_gui.ShowEffects);
        }
        public bool TryGetGestureFrame(string name, out GestureFrame gestureFrame)
        {
            var id = GestureMapper.IndexOfName(name);
            gestureFrame = id < frames.Count ? frames[id] : null;
            return id < frames.Count;
        }
    }
}