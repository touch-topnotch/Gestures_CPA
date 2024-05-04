using System.Collections.Generic;
using Scripts.Design;
using Scripts.PlayerLogic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters
{
    
    [InlineEditor()]
    [CreateAssetMenu(fileName = "CharData_", menuName = "Character/CharacterData")]
    public class CharacterData: SerializedScriptableObject
    {
        public string characterName = "";
        public Dictionary<AvatarType, GameObject> avatars = new Dictionary<AvatarType, GameObject>();
        public Dictionary<string, GameObject> weapons = new Dictionary<string, GameObject>();
        
        [ShowInInspector]
        public HandAppearance handAppearance;
    }
}