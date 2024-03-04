using System;
using System.Collections.Generic;
using Gesture_Editor_SDK.EditorAttributes.InspectorButtonAttribute;
using Gesture_Editor_SDK.ReadOnly;
using Gesture_Editor_SDK.Realtime;
using Scripts.Databases;
using Scripts.Design;
using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.PlayerLogic;
using Scripts.Tests;
using UnityEngine;
using UnityEngine.Rendering;
using Avatar = Scripts.PlayerLogic.Avatar;


namespace Scripts.Characters
{
    public class Character : MonoBehaviour
    {
        [SerializeField]
        private CustomDictionary<AvatarType, Avatar> _avatarsDictionary =
            new CustomDictionary<AvatarType, Avatar>();
        
        public HandAppearanceProcessor handAppearance;
        public List<IRecognizable> recognizables { get; private set; } = new List<IRecognizable>();
        
        private AvatarType _currentType;

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent(typeof(IRecognizable), out var component))
                {
                    recognizables.Add(component as IRecognizable);
                }
            }
        }

        public Avatar GetAvatar()
        {
            if (!_avatarsDictionary.ContainsKey(GetAvatarType) || GetAvatarType == AvatarType.None)
            {
                return null;
            }
            
            
            return _avatarsDictionary[GetAvatarType];
        }

        public AvatarType GetAvatarType => _currentType;
        public void ChangeAvatarType(AvatarType type, Hands hands)
        {
            _currentType = type;
            
            if (transform.childCount != _avatarsDictionary.Count)
            {
                FindAvatars();
            }
            
            foreach (Avatar avatar in _avatarsDictionary.Values)
            {
               // Debug.Log(avatar.gameObject.name + "  " + (avatar.type == type).ToString());
                avatar.gameObject.SetActive(avatar.type == type);
            }
            
            if(hands != null)
                ChangeMaterials(hands.HandMaterialPair, _currentType);
        }

        public void FindAvatars()
        {
            foreach (var VARIABLE in transform.GetComponentsInChildren<Avatar>())
            {
                if (_avatarsDictionary.ContainsKey(VARIABLE.type))
                    _avatarsDictionary[VARIABLE.type] = VARIABLE;
                else
                    _avatarsDictionary.Add(VARIABLE.type, VARIABLE);
                
            }
        }

        public void ChangeMaterials(MaterialPair materialPair)
        {
            handAppearance.ChangeMaterialPair(materialPair, _currentType);
        }
        public void ChangeMaterials(MaterialPair materialPair, AvatarType type)
        {
            if(handAppearance)
                handAppearance.ChangeMaterialPair(materialPair, type);
        }
    }
}
