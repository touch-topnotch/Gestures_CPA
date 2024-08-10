using System.Collections;
using System.Collections.Generic;
using Scripts.Design;
using Scripts.Gestures;
using UnityEngine;
using UnityEngine.UI;

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
