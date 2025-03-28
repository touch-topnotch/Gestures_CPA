using System.Collections.Generic;
using Characters;
using Gesture_Editor_SDK.Realtime;
using Scripts.Design;
using Scripts.HandsLogic;
using Scripts.PlayerLogic;
using Scripts.Static;
using Scripts.Systems;
using Scripts.Tests;
using Scripts.Weapons;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Avatar = Scripts.PlayerLogic.Avatar;


namespace Scripts.Characters
{
    public class Character: MonoBehaviour
    {
    
        private readonly Dictionary<AvatarType, Avatar> _avatarsDictionary = new ();
        private readonly Dictionary<string, Weapon> _weapons = new Dictionary<string, Weapon>();

        private AvatarType _currentType;
        private HandAppearanceProcessor _handAppearanceProcessor;
        public void SetSource(CharacterData data)
        {
            foreach (var VARIABLE in data.avatars)
            {
                if (_avatarsDictionary.ContainsKey(VARIABLE.Key))
                {
                    Debug.LogWarning("Overwriting avatar " + VARIABLE.Key);
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

            if (data.weapons != null)
            {
                foreach (var WEAPON in data.weapons)
                {
                    if (WEAPON.Value == null)
                    {
                        continue;
                    }

                    if (WEAPON.Value.GetComponent<Weapon>() == null)
                    {
                        continue;
                    }

                    if (_weapons.ContainsKey(WEAPON.Key))
                    {
                        Debug.LogWarning("Overwriting weapon " + WEAPON.Key);
                        Destroy(_weapons[WEAPON.Key].gameObject);
                        _weapons[WEAPON.Key] = Instantiate(WEAPON.Value, this.transform).GetComponent<Weapon>();
                    }
                    else
                    {
                        _weapons.Add(WEAPON.Key, Instantiate(WEAPON.Value, this.transform).GetComponent<Weapon>());
                    }
                }
            }

            _handAppearanceProcessor = new HandAppearanceProcessor(data.handAppearance);
        }

        public Avatar GetAvatar()
        {
            if (!_avatarsDictionary.ContainsKey(getAvatarType) || getAvatarType == AvatarType.None)
            {
                return null;
            }
            return _avatarsDictionary[getAvatarType];
        }

        public Dictionary<string,Weapon> weapons => _weapons;
        
        public AvatarType getAvatarType => _currentType;
        public void ChangeAvatarType(AvatarType type, Hands hands)
        {
            _currentType = type;

            foreach (Avatar avatar in _avatarsDictionary.Values)
            {
               // Debug.Log(avatar.gameObject.name + "  " + (avatar.type == type).ToString());
                avatar.gameObject.SetActive(avatar.type == type);
            }
            
            if(hands != null)
                ChangeMaterials(hands.HandMaterialPair, _currentType);
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
