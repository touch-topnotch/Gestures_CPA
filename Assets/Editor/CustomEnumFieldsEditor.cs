using System.Collections;
using System.Collections.Generic;
using Scripts.Hands;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SupportHandCreator))]
public class CustomEnumFieldsEditor : Editor
{
    SerializedProperty type;
    SerializedProperty leftHandField;
    SerializedProperty rightHandField;
    SerializedProperty bonesField;
    SerializedProperty parentField;
    private void OnEnable()
    {
        type = serializedObject.FindProperty("type");
        leftHandField = serializedObject.FindProperty("LeftHand");
        rightHandField = serializedObject.FindProperty("RightHand");
        bonesField = serializedObject.FindProperty("Bones");
        parentField = serializedObject.FindProperty("Parent");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(type);
        

        if (type.enumValueIndex == 0) // Corresponds to Type.A
        {
            EditorGUILayout.PropertyField(leftHandField);
            EditorGUILayout.PropertyField(rightHandField);
        }
        else
        {
            EditorGUILayout.PropertyField(bonesField);
        }
        EditorGUILayout.PropertyField(parentField);
        serializedObject.ApplyModifiedProperties();
    }
}