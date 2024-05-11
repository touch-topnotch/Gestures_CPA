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
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using Avatar = Scripts.PlayerLogic.Avatar;


namespace Scripts.Characters
{
    public class Character : MonoBehaviour
    {
        private readonly Dictionary<AvatarType, Avatar> _avatarsDictionary = new();
        private readonly Dictionary<string, Weapon> _weapons = new Dictionary<string, Weapon>();
        private AvatarType _currentType = AvatarType.None;
        private HandAppearanceProcessor _handAppearanceProcessor;
        public void SpawnCharacters(CharacterData data)
        {
            Debug.Log("SETTING SOURCE " + data.characterName);
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

            _handAppearanceProcessor = new HandAppearanceProcessor(data.handAppearance);
        }
        public void SetWeapons(in List<ulong> weapons)
        {
            _weapons.Clear();
            foreach (var weapon_ulong in weapons)
            {
                var nO = NetworkManager.Singleton.SpawnManager.SpawnedObjects[weapon_ulong].GetComponent<Weapon>();
                _weapons.Add(nO.name.Split('_')[0], nO);
            }

            Debug.Log("Character " + name + " contains " + Debugger.dictionaryToString(_weapons, false, true));
        }
        public List<ulong> SpawnWeapons(in Dictionary<string, GameObject> weapons, PlayerData data)
        {
            var spawns = new List<ulong>();
            if (weapons != null)
            {
                foreach (var WEAPON in weapons)
                {
                    if (WEAPON.Value == null)
                    {
                        continue;
                    }

                    if (WEAPON.Value.GetComponent<Weapon>() == null)
                    {
                        continue;
                    }

                    var instance = Instantiate(WEAPON.Value).GetComponent<NetworkObject>();
                    instance.Spawn();
                    if (!instance.TrySetParent(this.transform.parent.parent))
                    {
                        Debug.Log("Can't set parent for " + WEAPON.Key);
                    }

                    if (_weapons.ContainsKey(WEAPON.Key))
                    {
                        Debug.Log("Overwriting weapon " + WEAPON.Key);
                        Destroy(_weapons[WEAPON.Key].gameObject);
                        _weapons[WEAPON.Key] = instance.GetComponent<Weapon>();
                    }
                    else
                    {
                        Debug.Log("Adding weapon " + WEAPON.Key);
                        _weapons.Add(WEAPON.Key, instance.GetComponent<Weapon>());
                    }

                    //_weapons[WEAPON.Key].Initialize()
                    spawns.Add(_weapons[WEAPON.Key].NetworkObjectId);
                }
            }

            Debug.Log("Character " + name + " contains " + Debugger.dictionaryToString(_weapons, false, true));
            return spawns;
        }
        public Avatar curAvatar => _currentType == AvatarType.None ? null : _avatarsDictionary[getAvatarType];
        public Dictionary<string, Weapon> weapons => _weapons;
        public AvatarType getAvatarType => _currentType;
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