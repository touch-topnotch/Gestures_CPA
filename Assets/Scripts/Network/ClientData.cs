using UnityEngine;

namespace Scripts.Network
{
    public enum Skill
    {
        LOOSER,
        MEDIUM,
        PRO
    }
    public class ClientData: ScriptableObject
    {
        public string name;
        public string tag;
        public string iconPath;
        public Skill skill;
    }
}