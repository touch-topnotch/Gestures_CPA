using Scripts.Hands;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HandMesh))]
public class HandMeshEditor: Editor
{  
    private SerializedProperty _handMaterialType;
    private SerializedProperty _handType;
    private SerializedProperty points;
    private SerializedProperty material;
    private SerializedProperty _defaultMaterial;
    
    private void OnEnable()
    {
        _handMaterialType = serializedObject.FindProperty("_handMaterialType");
        _handType = serializedObject.FindProperty("_handType");
        points = serializedObject.FindProperty("points");
        material = serializedObject.FindProperty("material");
        _defaultMaterial = serializedObject.FindProperty("_defaultMaterial");
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_handMaterialType);
        EditorGUILayout.PropertyField(_handType);
        EditorGUILayout.PropertyField(points);
        EditorGUILayout.PropertyField(material);
        EditorGUILayout.PropertyField(_defaultMaterial);
        GUILayout.Space(20);
        if (GUILayout.Button("Add missing components"))
        {
            OnClick();
        }
        serializedObject.ApplyModifiedProperties();
    }


    private void OnClick()
    {
        // get HandMesh component
        HandMesh mesh = (HandMesh)target;
        mesh.RefreshProperties();
    }
}