using UnityEditor;
using UnityEngine;

namespace Gesture_Editor_SDK.EditorAttributes.SerializeByTypeAttribute.Editor
{
  
// Custom Property Drawer
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SerializeByTypeAttribute))]
    public class SerializeIfEnumDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = (SerializeByTypeAttribute)this.attribute;
            if (attribute.enumField != null && attribute.enumType.IsEnum)
            {
                if (attribute.enumValueIndex != -1)
                {
                    var enumProperty =
                        property.serializedObject.FindProperty(
                            property.propertyPath.Replace(property.name, attribute.enumField));
                    if (enumProperty != null && enumProperty.enumValueIndex == attribute.enumValueIndex)
                    {
                        EditorGUI.PropertyField(position, property, label);
                    }
                }
            }
        }
    }
#endif
}
