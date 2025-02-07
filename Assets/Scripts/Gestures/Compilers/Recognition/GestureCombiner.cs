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

        public FrameRecognized OnFrameRecognized => _recognizer.onFrameRecognized;
        public GestureRecognized OnGestureRecognized => _recognizer.onGestureRecognized;
        
        public void Initialize(Rig rig, bool debugMode)
        {
            _recognizer = new Recognizer(rig.Hands, rig.RecognitionPropertiesConfig);
            if(debugMode)
                RecognizeWithAllGestures();
        }
        private void RecognizeWithAllGestures()
        {
            _recognizer.RecognizeDynamicGesture(GesturesLibrary.Instance.DynamicGestures);
        } 

        public void GestureRecognized(DynamicGesture gesture)
        {
            Debug.Log($"Dynamic gesture {gesture.Name} recognized");
            
            gesture.AllFramesDetected(OnTheEndOfGestureCall);
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

            GestureGraphManager.InitializeGestureGraph(GesturesLibrary.Instance.DynamicGestures);

        }
    }
}