using Scripts.Gestures;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Design
{
    public class ToggleContainer : MonoBehaviour
    {

        public CreatedGesturesVisualizer Visualizer;
        public DynamicGesture Gesture;
    
        public void OnToggleClicked(bool isOn)
        {
            if(isOn)
                Visualizer.ShowDynamicGesture(Gesture, GetComponent<Toggle>());
        }
    }
}
