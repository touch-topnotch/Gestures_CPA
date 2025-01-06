using Scripts.Hands;
using UnityEditor;
using UnityEngine;

public class OptionalHandsCreatorEditor : Editor
{
    // SerializedProperty leftHandField;
    // SerializedProperty rightHandField;
    // SerializedProperty hasCreatorField;
    // SerializedProperty handCreatorField;
    //
    // private void OnEnable()
    // {
    //     leftHandField = serializedObject.FindProperty("leftHand");
    //     rightHandField = serializedObject.FindProperty("rightHand");
    //     hasCreatorField = serializedObject.FindProperty("haveCreator");
    //     handCreatorField = serializedObject.FindProperty("handCreator");
    // }
    //
    // public override void OnInspectorGUI()
    // {
    //     serializedObject.Update();
    //
    //     EditorGUILayout.PropertyField(leftHandField);
    //     EditorGUILayout.PropertyField(rightHandField);
    //     EditorGUILayout.PropertyField(hasCreatorField);
    //     if (hasCreatorField.boolValue) 
    //     {
    //         EditorGUILayout.PropertyField(handCreatorField);
    //     }
    //     serializedObject.ApplyModifiedProperties();
    // }
}

