using System;
using UnityEngine;

namespace Scripts.UI
{
    [Serializable]
    [CreateAssetMenu(fileName = "Palette_", menuName = "Config/Palette")]
    public class Palette : ScriptableObject
    {
        public Color active = Color.green;
        public Color enabled = Color.yellow;
        public Color wrong = Color.red;
        public Color clear = Color.white;
        public Color disabled = Color.gray;
    }
}