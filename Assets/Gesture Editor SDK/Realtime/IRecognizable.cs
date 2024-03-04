
using System;
using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.PlayerLogic;
using UnityEngine.Events;

namespace Gesture_Editor_SDK.Realtime
{
    public interface IRecognizable
    {
        string gestureName { get; }
        PlayerData playerData {get; set; }
        void OnFrameRecognized(string name);
        void AbilityCalled();
        void AbilityReleased();
        event Action AbilityReleasedEvent;
    }
}
