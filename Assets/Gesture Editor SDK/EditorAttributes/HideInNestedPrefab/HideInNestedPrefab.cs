using System;
using UnityEditor;

namespace Gesture_Editor_SDK.EditorAttributes
{
 
//     public class DisableInNestedPrefabAttribute : PropertyAttribute
//     {
//         public string title;
//
//         public DisableInNestedPrefabAttribute(string title ="This property should be changed only inside the prefab!" )
//         {
//             this.title = title;
//         }
//     }
// #if UNITY_EDITOR
//     [UnityEditor.CustomPropertyDrawer(typeof(DisableInNestedPrefabAttribute))]
//     public class DisableInNestedPrefabDrawer : UnityEditor.PropertyDrawer
//     {
//         private bool ShouldDisableProperty(SerializedProperty property)
//         {
//             
//             var prefabInstance = UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(property.serializedObject.targetObject);
//             if (prefabInstance != null)
//             {
//                 var prefabSource = UnityEditor.PrefabUtility.GetOutermostPrefabInstanceRoot(prefabInstance);
//                 return prefabSource == null;
//             }
//             return false;
//         }
//
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             DisableInNestedPrefabAttribute disableInNestedPrefabAttribute = (DisableInNestedPrefabAttribute)attribute;
//             
//             if (ShouldDisableProperty(property))
//             {
//                 Rect tooltipRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
//                 EditorGUI.HelpBox(tooltipRect, "This property should be changed only inside the prefab!", UnityEditor.MessageType.Info);
//                 //    EditorGUI.PropertyField(position, property, label, true);
//             }
//             else
//             {
//                 EditorGUI.PropertyField(position, property, label, true);
//             }
//         }
//
//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             if (ShouldDisableProperty(property))
//             {
//                 return EditorGUIUtility.singleLineHeight;// + base.GetPropertyHeight(property, label) + 5;
//             }
//
//             return base.GetPropertyHeight(property, label);
//         }
//     }
// #endif

}
