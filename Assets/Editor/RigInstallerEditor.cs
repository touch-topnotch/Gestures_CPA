using Scripts.Installers;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RigInstaller))]
public class RigInstallerEditor: Editor
{
    private SerializedProperty instantiate;
    private SerializedProperty type;
    private SerializedProperty rig;
    private void OnEnable()
    {
        type = serializedObject.FindProperty("type");
        instantiate = serializedObject.FindProperty("instantiate");
        rig = serializedObject.FindProperty("spawnedRig");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(instantiate);
        
        if (instantiate.boolValue) // Corresponds to Type.A
        {
            EditorGUILayout.PropertyField(type);
        }
        else
        {
            EditorGUILayout.PropertyField(rig);
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
