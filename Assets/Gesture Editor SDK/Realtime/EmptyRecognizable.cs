using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.PlayerLogic;
using Scripts.Static.Definitions;
using UnityEngine;
using UnityEngine.Events;

namespace Gesture_Editor_SDK.Realtime
{
    public class EmptyRecognizable : RecognizableBehaviour
    {
        public override AbilityType type => AbilityType.Character;

        public override void Initialize()
        {
            
        }

        public override void ReadyToBeRecognized()
        {
           // throw new System.NotImplementedException();
        }

        public override void OnFrameRecognized(string name)
        {
            Debug.Log("Executed empty gesture on frame: " + name);
        }

        public override void OnGestureCasted()
        {
            AbilityReleasedEvent.Invoke();
        }

        protected override void OnAbilityReleased()
        {
            
        }
    }
}