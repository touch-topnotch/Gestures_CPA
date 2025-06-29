using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Static
{
    public class SmartDict<T0, T1> : Dictionary<T0, T1>
    {
        public virtual void AddReplace(T0 key, T1 value, bool showCollisionLog = false)
        {
            if (this.ContainsKey(key))
            {
                this[key] = value;
                if (showCollisionLog)
                {
                    Debug.Log($"Dictionary already contains key {key}!");
                }
            }
            else
            {
                this.Add(key, value);
            }
        }
    } 
}