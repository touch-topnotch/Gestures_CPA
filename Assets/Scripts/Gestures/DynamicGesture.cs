using System.Collections.Generic;
using Gesture_Editor_SDK.Realtime;
using Scripts.Events;
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
        
        private readonly FrameDetected onFrameDetected = new();
        private IRecognizable _recognizable;

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
        public DynamicGesture(string name, List<GestureFrame> frames, IRecognizable recognizable)
        {
            Name = name;
            this.frames = frames;

            _recognizable = recognizable;
        }

        public DynamicGesture(string name)
        {
            Name = name;
            frames = new();
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
            _recognizable.AbilityCalled();
            _recognizable.OnAbilityReleased.AddListener(() =>
            {
                // Start to recognize next gesture
            });
            _currentGesture = 0;
            
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
            // CHANGE IT TO NEW SYSTEM WITH RECOGNIZABLE
            // if (_gui == null)
            //     return;
            // _gui.Construct(hands);
            onFrameDetected.AddListener(_recognizable.OnFrameRecognized);
        }
        public bool TryGetGestureFrame(string name, out GestureFrame gestureFrame)
        {
            var id = GestureMapper.IndexOfName(name);
            gestureFrame = id < frames.Count ? frames[id] : null;
            return id < frames.Count;
        }
    }
}