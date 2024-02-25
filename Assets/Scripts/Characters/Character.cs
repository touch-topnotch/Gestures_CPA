using System;
using System.Collections.Generic;
using Gesture_Editor_SDK.EditorAttributes.InspectorButtonAttribute;
using Gesture_Editor_SDK.ReadOnly;
using Scripts.Design;
using Scripts.HandsLogic;
using Scripts.PlayerLogic;
using Scripts.Tests;
using UnityEngine;
using UnityEngine.Rendering;
using Avatar = Scripts.PlayerLogic.Avatar;


namespace Scripts.Characters
{
    [RequireComponent(typeof(HandAppearance))]
    public class Character : MonoBehaviour
    {
        [SerializeField]
        private CustomDictionary<AvatarType, Avatar> _avatarsDictionary =
            new CustomDictionary<AvatarType, Avatar>();

        [SerializeField] private GameObject _source;
        
        [SerializeField] private HandAppearance _handAppearance;
        

        private AvatarType _currentType;

        public Avatar GetAvatar()
        {
            if (!_avatarsDictionary.ContainsKey(GetAvatarType) || GetAvatarType == AvatarType.None)
            {
                return null;
            }
            
            
            return _avatarsDictionary[GetAvatarType];
        }
        
    // region UnityMethods

    #if UNITY_EDITOR
        [InspectorButton("Generate Avatars")]
        public void GenerateAvatars()
        {
            
            // if source name includes _Character - find children with name Body, Head
            // then generate avatars by path Resources/Avatars/CharacterName
            // each avatar must have Avatar component
            // each avatar must have Head and Body fromm source
            // avatars should be saved by path Resources/Avatars/CharacterName/CharacterName_AvatarType

            var characterName = _source.name.Split('_')[0];
            Transform head = _source.transform.Find("Head");
            Transform body = _source.transform.Find("Body");
            string path = $"Assets/Resources/Characters/{characterName}/";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            foreach (var avatarType in Enum.GetNames(typeof(AvatarType)))
            {
                if(avatarType == "None")
                    continue;
                // create new GameObject
                // add Avatar component
                // add Head and Body from source
                // save as prefab

                var temporaryObject = new GameObject();
                var avatarPrefab = Instantiate(temporaryObject, this.transform);
             
                avatarPrefab.name = $"{characterName}_{avatarType}";
                var avatar = avatarPrefab.AddComponent<Avatar>();
                var anchors = avatarPrefab.AddComponent<BodyAnchors>();
                avatar.Anchors = anchors;
                avatar.GetComponent<Avatar>().type = (AvatarType) Enum.Parse(typeof(AvatarType), avatarType);
                // add Head
                
                
                var headClone = Instantiate(head, avatarPrefab.transform);
                headClone.name = "Head";
                avatar.Anchors.Head = headClone;
                headClone.gameObject.SetActive(avatarType != "Local");

                // add Body
                var bodyClone = Instantiate(body, avatarPrefab.transform);
                bodyClone.name = "Body";
                avatar.Anchors.Body = bodyClone;
                // save as prefab
                
         
                // if path isn't exists - create it
                UnityEditor.PrefabUtility.SaveAsPrefabAsset(avatarPrefab, $"{path}{avatar.name}.prefab");
                DestroyImmediate(temporaryObject);
            }
           
            FindAvatars();
            _handAppearance = GetComponent<HandAppearance>();
            
            // if prefab CharacterName_Character not exists in path - create it
            if(!System.IO.File.Exists($"{path}{characterName}.prefab")) 
                UnityEditor.PrefabUtility.SaveAsPrefabAsset(this.gameObject, $"{path}{characterName}.prefab");
        }
        
    #endif
        // endregion UnityMethods

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

        private void FindAvatars()
        {
            foreach (var VARIABLE in transform.GetComponentsInChildren<Avatar>())
            {
                if (_avatarsDictionary.ContainsKey(VARIABLE.type))
                    _avatarsDictionary[VARIABLE.type] = VARIABLE;
                else
                    _avatarsDictionary.Add(VARIABLE.type, VARIABLE);
                
            }
        }
        
        public void SetSource(GameObject value) => _source = value;

        public void ChangeMaterials(MaterialPair materialPair)
        {
            _handAppearance.ChangeMaterialPair(materialPair, _currentType);
        }
        public void ChangeMaterials(MaterialPair materialPair, AvatarType type)
        {
            if(_handAppearance)
                _handAppearance.ChangeMaterialPair(materialPair, type);
        }

    }
}
