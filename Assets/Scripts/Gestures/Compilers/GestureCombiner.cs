using System;
using Scripts.Events;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Scripts.Gestures
{
    
    [RequireComponent(typeof(Recognizer))]
    public class GestureCombiner: CustomBehaviour
    {
        [SerializeField] private bool activateOnAwake;
        [SerializeField] private Button button;
        
        private RecognitionEvent _onDynamicRecognized = new();
        private GesturesLibrary _library;
        private Recognizer _recognizer;
        private GestureGraph _graph;
        private GestureGraph _currentGraph;

        [Inject]
        public void Construct(GesturesLibrary library)
        {
            _library = library;
            _onDynamicRecognized.AddListener(GestureRecognized);
            
            if(activateOnAwake)
                TestRecognitionFunction();
            // else
            //     button.onClick.AddListener(TestRecognitionFunction);
        }

        private void OnValidate()
        {
            _recognizer = GetComponent<Recognizer>();
        }
        public void TestRecognitionFunction()
        {
            _recognizer.RecognizeDynamicGesture(_library.DynamicGestures, ref _onDynamicRecognized);
        }

        public void GestureRecognized(DynamicGesture gesture)
        {
            Debug.Log($"Dynamic gesture {gesture.Name} recognized");
            
            gesture.AllFramesDetected();

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