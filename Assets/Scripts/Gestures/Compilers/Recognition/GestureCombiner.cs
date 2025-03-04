using System.Collections.Generic;
using Scripts.Characters;
using Scripts.Events;
using Scripts.PlayerLogic;
using UnityEngine;

namespace Scripts.Gestures
{
    public class GestureCombiner
    {
        private GestureGraph _graph;
        
        private GestureGraph _currentGraph; 
        
        private Recognizer _recognizer;

        public readonly GesturesLibrary library;
        public FrameRecognized OnFrameRecognized => _recognizer.onFrameRecognized;
        public GestureRecognized OnGestureRecognized => _recognizer.onGestureRecognized;
        private PlayerData _data;
        public GestureCombiner(PlayerData data, Dictionary<string, Character> chars)
        {
            _data = data;
            library = new GesturesLibrary(data, chars);
        }
        public void CreateRecognizer(RecognitionPropertiesConfig config, bool isDebug = true)
        {
            _recognizer = new Recognizer(_data.hands, config);
            
            if(isDebug)
                RecognizeWithAllGestures();
        }
        private void RecognizeWithAllGestures()
        {
            _recognizer.RecognizeDynamicGesture(library.DynamicGestures);
        } 

        public void GestureRecognized(DynamicGesture gesture)
        {
            Debug.Log($"Dynamic gesture {gesture.Name} recognized");
            
            gesture.AllFramesDetected(OnTheEndOfGestureCall);
        }

        public void SimulateFrame(string name)
        {
            
            if(library.TryGetDynamicGesture(name,out var gesture))
            {
                gesture.FrameRecognized(name);
                
                if(gesture.TryGetGestureFrame(name, out var frame))
                {
                    Debug.Log("Move hands");
                    _data.hands.MoveHands(frame, 4, () => { Debug.Log("Frame Simulated!"); });
                }
            }
        }

        public void SimulateGesture(string name)
        {
            if(library.TryGetDynamicGesture(name,out var gesture))
            {
                gesture.AllFramesDetected(()=>{});
            }
        }
        public void OnTheEndOfGestureCall()
        {
            RecognizeWithAllGestures();
        }

        
        // in Network Player [ServerRpc]
        private void CreateCombination()
        {
            // if (state != GameState.Fight)
            // {
            //     return;
            // }

            GestureGraphManager.InitializeGestureGraph(library.DynamicGestures);

        }
    }
}