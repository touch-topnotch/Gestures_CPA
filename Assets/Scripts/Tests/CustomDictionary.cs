using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts.Tests
{
    public class EditableDictionary<K, V> : Dictionary<K, V>, IPrefabDictionaryEditable<K, V>
    {
        public Dictionary<K, V> GetDictionaryInEditMode()
        {
            return this;
        }

        public void SetDictionaryInEditMode(Dictionary<K, V> dict)
        {
            Clear();
            foreach (var key in dict.Keys)
            {
                Add(key, dict[key]);
            }
        }
    }

    public interface IPrefabDictionaryEditable<K, V>
    {
        public Dictionary<K, V> GetDictionaryInEditMode();
        public void SetDictionaryInEditMode(Dictionary<K, V> dict);
    }

    /// <summary>
    /// Unity can't serialize Dictionary so here's a custom wrapper that does. Note that you have to
    /// extend it before it can be serialized as Unity won't serialized generic-based types either.
    /// </summary>
    /// <typeparam name="K">The key type</typeparam>
    /// <typeparam name="V">The value</typeparam>
    /// <example>
    /// public sealed class MyDictionary : SerializedDictionary&lt;KeyType, ValueType&gt; {}
    /// </example>
    [Serializable]
    public class CustomDictionary<K, V> : CustomDictionary<K, V, K, V>
    {
        /// <summary>
        /// Conversion to serialize a key
        /// </summary>
        /// <param name="key">The key to serialize</param>
        /// <returns>The Key that has been serialized</returns>
        public override K SerializeKey(K key) => key;

        /// <summary>
        /// Conversion to serialize a value
        /// </summary>
        /// <param name="val">The value</param>
        /// <returns>The value</returns>
        public override V SerializeValue(V val) => val;


        public override V SerializeValue(List<V> value)
        {
            List<object> serializedList = new List<object>();

            foreach (var item in value)
            {
                serializedList.Add(SerializeValue(item));
            }

            return (V)(object)serializedList;
        }


        /// <summary>
        /// Conversion to serialize a key
        /// </summary>
        /// <param name="key">The key to serialize</param>
        /// <returns>The Key that has been serialized</returns>
        public override K DeserializeKey(K key) => key;

        /// <summary>
        /// Conversion to serialize a value
        /// </summary>
        /// <param name="val">The value</param>
        /// <returns>The value</returns>
        public override V DeserializeValue(V val) => val;

        public void SmartAdd(K key, V val)
        {
            if (ContainsKey(key))
            {
                this[key] = val;
            }
            else
            {
                Add(key, val);
            }
        }
#if UNITY_EDITOR
        public static Dictionary<K, V> GetDictionaryFromPrefab(string path)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
                throw new NullReferenceException();

            if (prefab.TryGetComponent(typeof(IPrefabDictionaryEditable<K, V>), out var component))
            {
                // Change a property of the component
                var prefabDictEditable = (IPrefabDictionaryEditable<K, V>)component;
                return prefabDictEditable.GetDictionaryInEditMode();
            }

            throw new NullReferenceException();
        }

        public static void SetDictionaryToPrefab(string path, Dictionary<K, V> dict)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null)
                throw new NullReferenceException();

            GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

            if (prefabInstance == null)
                throw new NullReferenceException();

            if (prefabInstance.TryGetComponent(typeof(IPrefabDictionaryEditable<K, V>), out var component))
            {
                var prefabDictEditable = (IPrefabDictionaryEditable<K, V>)component;
                prefabDictEditable.SetDictionaryInEditMode(dict);

                PrefabUtility.ApplyPrefabInstance(prefabInstance, InteractionMode.AutomatedAction);
                Object.DestroyImmediate(prefabInstance);
                AssetDatabase.Refresh();
            }
            else
            {
                throw new NullReferenceException();
            }
        }
#endif
    }

    /// <summary>
    /// Dictionary that can serialize keys and values as other types
    /// </summary>
    /// <typeparam name="K">The key type</typeparam>
    /// <typeparam name="V">The value type</typeparam>
    /// <typeparam name="SK">The type which the key will be serialized for</typeparam>
    /// <typeparam name="SV">The type which the value will be serialized for</typeparam>
    [Serializable]
    public abstract class CustomDictionary<K, V, SK, SV> : Dictionary<K, V>, ISerializationCallbackReceiver
    {
        [SerializeField] List<SK> m_Keys = new List<SK>();

        [SerializeField] List<SV> m_Values = new List<SV>();

        /// <summary>
        /// From <see cref="K"/> to <see cref="SK"/>
        /// </summary>
        /// <param name="key">They key in <see cref="K"/></param>
        /// <returns>The key in <see cref="SK"/></returns>
        public abstract SK SerializeKey(K key);

        /// <summary>
        /// From <see cref="V"/> to <see cref="SV"/>
        /// </summary>
        /// <param name="value">The value in <see cref="V"/></param>
        /// <returns>The value in <see cref="SV"/></returns>
        public abstract SV SerializeValue(V value);

        public abstract SV SerializeValue(List<V> value);

        /// <summary>
        /// From <see cref="SK"/> to <see cref="K"/>
        /// </summary>
        /// <param name="serializedKey">They key in <see cref="SK"/></param>
        /// <returns>The key in <see cref="K"/></returns>
        public abstract K DeserializeKey(SK serializedKey);

        /// <summary>
        /// From <see cref="SV"/> to <see cref="V"/>
        /// </summary>
        /// <param name="serializedValue">The value in <see cref="SV"/></param>
        /// <returns>The value in <see cref="V"/></returns>
        public abstract V DeserializeValue(SV serializedValue);


        /// <summary>
        /// OnBeforeSerialize implementation.
        /// </summary>
        public void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();
            foreach (var kvp in this)
            {
                m_Keys.Add(SerializeKey(kvp.Key));
                m_Values.Add(SerializeValue(kvp.Value));
            }
        }

        /// <summary>
        /// OnAfterDeserialize implementation.
        /// </summary>
        public void OnAfterDeserialize()
        {
            for (int i = 0; i < m_Keys.Count; i++)
            {
                var key = DeserializeKey(m_Keys[i]);

                //  Debug.Log("Deserialized value: " + m_Values[i]);

                if (m_Values.Count < i)
                    continue;
                if (ContainsKey(key))
                {
                    this[key] = DeserializeValue(m_Values[i]);
                }
                else
                {
                    Add(key, DeserializeValue(m_Values[i]));
                }
            }

            m_Keys.Clear();
            m_Values.Clear();
        }
    }
}