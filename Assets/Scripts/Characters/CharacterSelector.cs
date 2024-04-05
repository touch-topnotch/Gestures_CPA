using System;
using Scripts.Characters;
using Scripts.PlayerLogic;
using UnityEngine;

namespace Characters
{
   
    
    public class CharacterSelector: MonoBehaviour
    {
        enum DebugCharacters
        {
            Anger,
            Grief,
            Fearlessness
        }
        [SerializeField] private DebugCharacters currentCharacter;
        [SerializeField] private CharacterPool _characterPool;
        private void OnValidate()
        {
            if (_characterPool == null)
            {
                _characterPool = this.gameObject.GetComponentInChildren<CharacterPool>();
            }
        }

        private void Start()
        {
            _characterPool.SetCharacter(currentCharacter.ToString());
        }
    }
}