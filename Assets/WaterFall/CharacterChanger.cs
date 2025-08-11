using System.Collections;
using System.Collections.Generic;
using Characters;
using Scripts.Players;
using Scripts.Static.Definitions;
using UnityEngine;

namespace Scripts
{
    public class CharacterChanger
    {
        private PlayerData _playerData;
        private List<CharacterData> _characterConfigs;
        
        private int _currentCharacterIndex;
        public int CurrentCharacterIndex
        {
            get => _currentCharacterIndex;
            private set
            {
                if (value < 0) 
                    _currentCharacterIndex = PlayerData.local.characterController.characterConfigs.Count - 1;
                else
                    _currentCharacterIndex = value % PlayerData.local.characterController.characterConfigs.Count;
            }
        }

        public CharacterChanger(string currentName)
        {
            _playerData = PlayerData.local;
            _characterConfigs = _playerData.characterController.characterConfigs;

            for (int i = 0; i < _characterConfigs.Count; i++)
            {
                if (_characterConfigs[i].characterName == currentName)
                {
                    CurrentCharacterIndex = i;
                }
            }
        }

        public GameObject SelectNextCharacter()
        {
            CurrentCharacterIndex++;
            return SelectCurrentCharacter();
        }
        
        public GameObject SelectPreviousCharacter()
        {
            CurrentCharacterIndex--;
            return SelectCurrentCharacter();
        }
        
        public GameObject SelectCurrentCharacter()
        {
            return _characterConfigs[_currentCharacterIndex].avatars[AvatarType.Enemy];
        }

        public void SetCharacter()
        {
            _playerData.characterController.SetCharacter(_characterConfigs[CurrentCharacterIndex].characterName);
        }
    }
}
