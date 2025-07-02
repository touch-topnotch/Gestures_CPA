using System.Collections.Generic;
using Characters;
using Gesture_Editor_SDK.Realtime;
using Scripts.Design;
using Scripts.HandsLogic;
using Scripts.PlayerLogic;
using Scripts.Static;
using Scripts.Static.Definitions;
using Scripts.Systems;
using Scripts.Tests;
using Scripts.Weapons;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using Avatar = Scripts.PlayerLogic.Avatar;


namespace Scripts.Characters
{
    public class Character : MonoBehaviour
    {
        private readonly Dictionary<AvatarType, Avatar> _avatarsDictionary = new();
        private AvatarType _currentType = AvatarType.None;
        private HandAppearanceProcessor _handAppearanceProcessor;
        public Avatar curAvatar => _currentType == AvatarType.None ? null : _avatarsDictionary[getAvatarType];
        public AvatarType getAvatarType => _currentType;
        public void SpawnCharacter(CharacterData data)
        {
            foreach (var VARIABLE in data.avatars)
            {
                if (_avatarsDictionary.ContainsKey(VARIABLE.Key))
                {
                    Destroy(_avatarsDictionary[VARIABLE.Key].gameObject);
                    _avatarsDictionary[VARIABLE.Key] =
                        Instantiate(VARIABLE.Value, this.transform).GetComponent<Avatar>();
                }
                else
                {
                    _avatarsDictionary.Add(VARIABLE.Key,
                        Instantiate(VARIABLE.Value, this.transform).GetComponent<Avatar>());
                }
            }

            _handAppearanceProcessor = new HandAppearanceProcessor(data.handAppearance);
        }

        public void ChangeAvatarType(AvatarType type, Hands hands)
        {
            _currentType = type;
            RefreshAvatars();
            if (hands != null)
                ChangeMaterials(hands.HandMaterialPair, _currentType);
        }

        public void RefreshAvatars()
        {
            foreach (Avatar avatar in _avatarsDictionary.Values)
            {
                avatar.gameObject.SetActive(avatar.type == _currentType);
            }
        }

        public void ChangeMaterials(MaterialPair materialPair)
        {
            _handAppearanceProcessor?.ChangeMaterialPair(materialPair, _currentType);
        }

        public void ChangeMaterials(MaterialPair materialPair, AvatarType type)
        {
            _handAppearanceProcessor?.ChangeMaterialPair(materialPair, type);
        }
    }
}