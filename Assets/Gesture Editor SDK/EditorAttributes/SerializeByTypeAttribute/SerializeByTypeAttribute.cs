using System;
using UnityEngine;


// Custom Attribute Definition
namespace Gesture_Editor_SDK.EditorAttributes.SerializeByTypeAttribute
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class SerializeByTypeAttribute : PropertyAttribute


    {
        public Type enumType;
        public string enumField;
        public int enumValueIndex;

        // conversations: 
        // 1. All path to enum class. Example: Scripts.Hands.HandMaterialType
        // 2. Field name = enum name with first letter in lower case. Example: handMaterialType
        public SerializeByTypeAttribute(string enumNameWithLib, string enumValue)
        {
            this.enumType = System.Type.GetType(enumNameWithLib +
                                                ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            var n = enumNameWithLib.Split(".")[^1];
            this.enumField = n[0].ToString().ToLower() + n.Substring(1);
            this.enumValueIndex = Array.IndexOf(Enum.GetNames(enumType), enumValue);
        }

        public SerializeByTypeAttribute(string enumNameWithLib, int enumValue)
        {
            this.enumType = System.Type.GetType(enumNameWithLib +
                                                ", Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            var n = enumNameWithLib.Split(".")[^1];
            this.enumField = n[0].ToString().ToLower() + n.Substring(1);
            this.enumValueIndex = enumValue;
        }
    }
}