using Scripts.Events;
using Scripts.PlayerLogic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Zenject;

namespace Scripts.Gestures
{
    [RequireComponent(typeof(Recognizer))]

    public class GestureCombiner
    {
        private RecognitionEvent _onDynamicRecognized = new RecognitionEvent();
        private GesturesLibrary _library;
        private Recognizer _recognizer;
        private GestureGraph _graph;
        private GestureGraph _currentGraph;
        
        
        [Inject]
        private void Construct(GesturesLibrary library, Recognizer recognizer)
        {
            _library = library;
            _recognizer = recognizer;
            _onDynamicRecognized.AddListener(GestureRecognized);
        }

        public void AddRecognitionButton(string name)
        {
            GameObject.Find(name).GetComponent<Button>().onClick.AddListener(TestRecognitionFunction);
        }

        public void TestRecognitionFunction()
        {
            _recognizer.RecognizeDynamicGesture(_library.DynamicGestures, ref _onDynamicRecognized);
        }

        public void GestureRecognized(DynamicGesture gesture)
        {
            Debug.Log($"Dynamic gesture {gesture.Name} recognized");
            gesture.AllFramesDetected();
            _recognizer.HideHands();
            
            TestRecognitionFunction();
        }

        private void CreateCombination()
        {
            // if (state != GameState.Fight)
            // {
            //     return;
            // }

            GestureGraphManager.InitializeGestureGraph(_library.DynamicGestures);

        }
    }
}