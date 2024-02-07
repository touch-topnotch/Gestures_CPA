using UnityEngine;

namespace Gesture_Editor_SDK.EditorAttributes.BooleanFieldAttribute
{
    // Custom Attribute Definition
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class SerializeIfAttribute : PropertyAttribute
    {
        public string condition;
        public SerializeIfAttribute(string condition)
        {
            this.condition = condition;
        }
    }
    
    // Custom Attribute Definition
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class SerializeIfNotAttribute : PropertyAttribute
    {
        public string condition;
        public SerializeIfNotAttribute(string condition)
        {
            this.condition = condition;
        }
    }

}
