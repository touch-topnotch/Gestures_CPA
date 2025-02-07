using Gesture_Editor_SDK.ReadOnly;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Gesture_Editor_SDK.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyInInspectorAttribute))]
    public class ReadOnlyInInspectorDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
}

#endif