using System.Collections;
using System.Collections.Generic;
using CrossPlatform.Gestures;
using CrossPlatform.Scripts.Design;
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
