using System;
using System.Collections.Generic;
using Gesture_Editor_SDK.Realtime;
using Scripts.HandsLogic;
using Scripts.Static;
using UnityEngine;

namespace Scripts.Gestures
{
    public enum GestureType
    {
        System,
        Weapon,
    }

    public class DynamicGesture // frame + IRecognizable = оружие
    {
        public List<FrameData> frames { get; }
        public GestureType gestureType { get; private set; }
        private IRecognizable _recognizable;

        private string _name;

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

        public DynamicGesture(string name, GestureType type, List<FrameData> frames, IRecognizable recognizable)
        {
            Name = name;
            gestureType = type;
            this.frames = frames;
            _recognizable = recognizable;
        }

        public DynamicGesture(string name)
        {
            Name = name;
            frames = new();
        }

        public bool HasFrame(string frame) => GestureMapper.PrefixOfName(frame) == Name &&
                                              GestureMapper.IndexOfName(frame) < frames.Count;

        public void AddFrame(FrameData frame)
        {
            frames.Add(frame);
        }

        //    public FrameData GetFrameData() => _currentGesture < frames.Count ? frames[_currentGesture] : null;

        public FrameData GetNextFrameOf(FrameData frame)
        {
            if (frame == null)
                return frames[0];

            var index = frames.IndexOf(frame);
            if (index == -1)
                return null;

            return index + 1 < frames.Count ? frames[index + 1] : null;
        }

        public void FrameRecognized(string name)
        {
            _recognizable?.OnFrameRecognized(name);
        }

        public void AllFramesDetected(Action onAbilityReleasedCallback)
        {
            if (_recognizable != null)
            {
                _recognizable.AbilityCalled();
                _recognizable.AbilityReleasedEvent += () => { onAbilityReleasedCallback?.Invoke(); };
            }
        }

        public bool TryGetFrameData(string name, out FrameData frameData)
        {
            var id = GestureMapper.IndexOfName(name);
            frameData = id < frames.Count ? frames[id] : null;
            return id < frames.Count;
        }

        public void LogFrames()
        {
            var log = $"Gesture {Name} contains: ";
            for (int i = 0; i < frames.Count; i++)
            {
                log += frames[i].name + " - base name: " + frames[i].baseName + ", ";
            }

            Debug.Log(log);
        }

        public FrameData this[int i]
        {
            get => frames[i];
        }
    }
}