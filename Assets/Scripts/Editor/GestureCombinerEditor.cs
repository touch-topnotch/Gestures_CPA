
using Scripts.Gestures;
using UnityEditor;

[CustomEditor(typeof(GestureCombiner))]
public class GestureCombinerEditor : Editor
{
    private SerializedProperty activateOnAwake;
    private SerializedProperty button;

    private void OnEnable()
    {
        activateOnAwake = serializedObject.FindProperty("activateOnAwake");
        button = serializedObject.FindProperty("button");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(activateOnAwake);

        if (activateOnAwake.boolValue == false) 
        {
            EditorGUILayout.PropertyField(button);
        }
        serializedObject.ApplyModifiedProperties();
    }
}