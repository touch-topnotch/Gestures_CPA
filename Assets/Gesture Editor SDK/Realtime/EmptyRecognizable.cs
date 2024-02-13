using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.PlayerLogic;
using UnityEngine;
using UnityEngine.Events;

namespace Gesture_Editor_SDK.Realtime
{
    public class EmptyRecognizable: RecognizableObject
    {
        public override void OnFrameRecognized(string name)
        {
            Debug.Log("Executed empty gesture on frame: " + name);
        }

        public override void AbilityCalled()
        {
            AbilityReleased();
        }

        protected override void OnAbilityReleased()
        {
            
        }
    }
}