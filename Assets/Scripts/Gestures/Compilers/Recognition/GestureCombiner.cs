using System.Collections.Generic;
using Scripts.Characters;
using Scripts.Events;
using Scripts.HandsLogic;
using Scripts.PlayerLogic;
using UnityEngine;

namespace Scripts.Gestures
{
    public class GestureCombiner : MonoBehaviour
    {
        public GesturesLibrary library;

        private FrameRecognized OnAbilityFrameRecognized;
        private GestureRecognized OnGestureRecognized;

        private GestureGraph _graph;

        private GestureGraph _currentGraph;

        private Recognizer _recognizer;


        public void Initialize(CharacterPool chars)
        {
            library = new GesturesLibrary(chars);
            OnGestureRecognized = new GestureRecognized();
            OnAbilityFrameRecognized = new FrameRecognized();
            OnAbilityFrameRecognized.AddListener((e) =>
            {
                library.characterGestures[GestureMapper.PrefixOfName(e)].FrameRecognized(e);
            });
            OnGestureRecognized.AddListener((e) =>
            {
                // start to recognize dynamic gestures again
                library.characterGestures[e].AllFramesDetected(RecognizeWithAllGestures);
            });
        }

        public void CreateRecognizer(RecognitionPropertiesConfig config)
        {
            _recognizer = new Recognizer(config);
        }

        public void RecognizeWithAllGestures()
        {
            StartCoroutine(_recognizer.RecognizeDynamicGesture(library.characterGestures, OnGestureRecognized,
                OnAbilityFrameRecognized));
        }

        // Simulate frame - is a specific function, which needs to simulate Hands movement on other (enemy) client device.
        public void SimulateFrame(PlayerHands hands, string name)
        {
            if (library.TryGetDynamicGesture(name, out var gesture))
            {
                gesture.FrameRecognized(name);

                if (gesture.TryGetGestureFrame(name, out var frame))
                {
                    Debug.Log("Move hands");
                    hands.MoveHands(frame, PlayerData.local.bodyAnchors, 4, () => { Debug.Log("Frame Simulated!"); },
                        true);
                }
            }
        }
    }
}