using System.Collections;
using System.Collections.Generic;
using Scripts.Players;
using Scripts.Static.Definitions;
using UnityEngine;

namespace Scripts
{
    public class CharacterChanger
    {
        private PlayerData _playerData;
        
        private int _currentCharacterIndex;
        private int CurrentCharacterIndex
        {
            get => _currentCharacterIndex;
            set
            {
                if (value < 0) 
                    _currentCharacterIndex = PlayerData.local.characterController.characterConfigs.Count - 1;
                else
                    _currentCharacterIndex = value % PlayerData.local.characterController.characterConfigs.Count;
            }
        }

        public CharacterChanger(int startIndex)
        {
            _playerData = PlayerData.local;
            CurrentCharacterIndex = startIndex;
        }

        public GameObject GetNextCharacter()
        {
            CurrentCharacterIndex++;
            return _playerData.characterController.characterConfigs[CurrentCharacterIndex]
                .avatars[AvatarType.Enemy];
        }
        
        public GameObject GetCurrentCharacter()
        {
            return _playerData.characterController.characterConfigs[CurrentCharacterIndex]
                .avatars[AvatarType.Enemy];
        }
        
        public GameObject GetPreviousCharacter()
        {
            CurrentCharacterIndex--;
            return _playerData.characterController.characterConfigs[CurrentCharacterIndex]
                .avatars[AvatarType.Enemy];
        }
    }
}
