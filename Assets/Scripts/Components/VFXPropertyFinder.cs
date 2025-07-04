using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.VFX;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Scripts
{
    public enum VFXPropertyType
    {
        Position,
        Rotation,
        Scale,
    }

    [Serializable]
    public class VFXProperty
    {
        public string name;
        public VFXPropertyType type = VFXPropertyType.Position;
        public Transform value;

        public VFXProperty(string name)
        {
            this.name = name;
        }
    }

    [ExecuteInEditMode]
    public class VFXPropertyFinder : MonoBehaviour
    {
        public List<VFXProperty> properties = new List<VFXProperty>();
        public VisualEffect vfx;
        public string AddNextComponents = "";
        [Button("Add Bindings")]
        public void AddBindings()
        {
            vfx ??= GetComponent<VisualEffect>();
            if (!vfx)
                return;
            if (AddNextComponents != "")
            {
                var comps = AddNextComponents.Split(' ');
                foreach (var comp in comps)
                {
                    var can = true;
                    foreach (var prop in properties)
                    {
                        if (prop.name == comp)
                            can = false;
                    }

                    if (can)
                        properties.Add(new VFXProperty(comp));
                }
            }

            AddNextComponents = "";
            foreach (var prop in properties)
            {
                prop.value = GameObject.Find(prop.name)?.transform;
            }
        }

        [Button("Add missing components")]

        public void AddMissingComponents()
        {
            foreach (var VARIABLE in properties)
            {
                if (VARIABLE.value == null)
                {
                    var t = GameObject.Find(VARIABLE.name);
                    if (t == null)
                        continue;
                    
                    VARIABLE.value = t.transform;
                }
            }
        }
        public void Update()
        {
            if (properties.Count == 0)
                return;
            foreach (var prop in properties)
            {
                if (prop.name == "" || prop.value == null || vfx.GetVector3(prop.name) == Vector3.zero)
                    continue;

                if (prop.type == VFXPropertyType.Position)
                {
                    vfx.SetVector3(prop.name, prop.value.transform.position);
                }

                if (prop.type == VFXPropertyType.Rotation)
                {
                    vfx.SetVector3(prop.name, prop.value.transform.rotation.eulerAngles);
                }

                if (prop.type == VFXPropertyType.Scale)
                {
                    vfx.SetVector3(prop.name, prop.value.transform.localScale);
                }
            }
        }
    }
// #if UNITY_EDITOR
//
//     [CustomEditor(typeof(VFXPropertyFinder))]
//     public class VFXPropertyFinderEditor : Editor
//     {
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
//             var finder = ((VFXPropertyFinder)target);
//             if (GUILayout.Button("Add Missing Components"))
//             {
//                 finder.AddBindings();
//             }
//
//             ((VFXPropertyFinder)target).Update();
//         }
//     }
// #endif
}