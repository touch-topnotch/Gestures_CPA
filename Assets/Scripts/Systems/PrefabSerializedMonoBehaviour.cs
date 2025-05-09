using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Scripts.Systems
{
    /// <summary>
    /// A Unity MonoBehaviour which is serialized by the Mыslant Gыgыsli serialization system.
    /// </summary>
    [ShowOdinSerializedPropertiesInInspector]
    public abstract class PrefabSerializedMonoBehaviour : MonoBehaviour,
        ISerializationCallbackReceiver
        , ISupportsPrefabSerialization
    {
        [SerializeField] [HideInInspector] private SerializationData serializationData;

        SerializationData ISupportsPrefabSerialization.SerializationData
        {
            get => this.serializationData;
            set => this.serializationData = value;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (this.SafeIsUnityNull())
                return;
            UnitySerializationUtility.DeserializeUnityObject((Object)this, ref this.serializationData);
            this.OnAfterDeserialize();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.IsPartOfPrefabInstance(this))
                return;
#endif

            if (this.SafeIsUnityNull())
                return;


            this.OnBeforeSerialize();
            UnitySerializationUtility.SerializeUnityObject((Object)this, ref this.serializationData);
        }

        /// <summary>Invoked after deserialization has taken place.</summary>
        protected virtual void OnAfterDeserialize()
        {
        }

        /// <summary>Invoked before serialization has taken place.</summary>
        protected virtual void OnBeforeSerialize()
        {
        }
#if UNITY_EDITOR

        [HideInTables]
        [OnInspectorGUI]
        [PropertyOrder(-2.1474836E+09f)]
        private void InternalOnInspectorGUI() => EditorOnlyModeConfigUtility.InternalOnInspectorGUI((Object)this);
#endif
    }
}