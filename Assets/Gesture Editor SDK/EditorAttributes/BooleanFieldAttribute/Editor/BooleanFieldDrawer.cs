using UnityEditor;
using UnityEngine;

namespace Gesture_Editor_SDK.EditorAttributes.BooleanFieldAttribute.Editor
{
    // Custom Property Drawer
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SerializeIfAttribute))]
    public class SerializeIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool enabled = GetCondition(property);
            bool wasEnabled = GUI.enabled;
            GUI.enabled = enabled;
            if (enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            GUI.enabled = wasEnabled;
        }

        private bool GetCondition(SerializedProperty property)
        {
            string conditionPath = property.propertyPath.Replace(property.name, (attribute as SerializeIfAttribute).condition);
            SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
            if (sourcePropertyValue != null)
            {
                return sourcePropertyValue.boolValue;
            }
            else
            {
                Debug.LogError("Property [" + (attribute as SerializeIfAttribute).condition + "] not found");
                return true;
            }
        }
    }
    
    [CustomPropertyDrawer(typeof(SerializeIfAttribute))]
    public class SerializeIfNotDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool enabled = !GetCondition(property);
            bool wasEnabled = GUI.enabled;
            GUI.enabled = enabled;
            if (enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
            GUI.enabled = wasEnabled;
        }

        private bool GetCondition(SerializedProperty property)
        {
            string conditionPath = property.propertyPath.Replace(property.name, (attribute as SerializeIfAttribute).condition);
            SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
            if (sourcePropertyValue != null)
            {
                return sourcePropertyValue.boolValue;
            }
            else
            {
                Debug.LogError("Property [" + (attribute as SerializeIfAttribute).condition + "] not found");
                return true;
            }
        }
    }
#endif
}
