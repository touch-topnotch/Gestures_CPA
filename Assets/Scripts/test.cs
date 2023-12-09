using System.Collections;
using System.Collections.Generic;
using Scripts.Events;
using Scripts.Gestures;
using UnityEngine;
using Zenject;

public class test : MonoBehaviour
{
    public Transform db_transform;
    public Transform hand_root;

    private void Update()
    {
        Debug.Log(hand_root.localRotation.eulerAngles);
       // Debug.Log(Recognizer.OptimizedDistance(db_transform.rotation, hand_root.rotation) + " rotation distance");
       // Debug.Log(Recognizer.OptimizedDistance(db_transform.position, hand_root.position) + " position distance");
    }
}
