using System;
using System.Collections.Generic;
using Gesture_Editor_SDK.EditorAttributes.InspectorButtonAttribute;
using Gesture_Editor_SDK.ReadOnly;
using Scripts.PlayerLogic;
using UnityEngine;
using Avatar = Scripts.PlayerLogic.Avatar;


namespace Scripts.Characters
{
    [Serializable]
    class AvatarCell
    {
        [SerializeField]
        
        [ReadOnlyInInspector]
    
        private AvatarType _type;

        public AvatarType type => _type;
        public Avatar avatar;

        public AvatarCell(AvatarType type)
        {
            this._type = type;
        }
    }
    public class Character : MonoBehaviour
    {
        [SerializeField] private AvatarType _currentType;
        [SerializeField] private List<AvatarCell> _avatars = new List<AvatarCell>();
        [SerializeField] private GameObject source;
        [SerializeField] private BodyAnchors ListenedAnchors;
        private readonly Dictionary<AvatarType, Avatar> _avatarsDictionary = new();
        
        
        public Avatar curAvatar => _avatarsDictionary[CurrentType];
        
        
        #if UNITY_EDITOR
        [InspectorButton("Generate Avatars")]
        private void GenerateAvatars()
        {
            // if source name includes _Character - find children with name Body, Head
            // then generate avatars by path Resources/Avatars/CharacterName
            // each avatar must have Avatar component
            // each avatar must have Head and Body fromm source
            // avatars should be saved by path Resources/Avatars/CharacterName/CharacterName_AvatarType

            var characterName = source.name.Split('_')[0];
            Transform head = source.transform.Find("Head");
            Transform body = source.transform.Find("Body");
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

                var avatarPrefab = Instantiate(new GameObject(), this.transform);
             
                avatarPrefab.name = $"{characterName}_{avatarType}";
                var avatar = avatarPrefab.AddComponent<Avatar>();
                avatar.GetComponent<Avatar>().type = (AvatarType) Enum.Parse(typeof(AvatarType), avatarType);
                // add Head
                if (avatarType != "Local")
                {
                    var headClone = Instantiate(head, avatarPrefab.transform);
                    headClone.name = "Head";
                    avatar.head = headClone;
                }

                // add Body
                var bodyClone = Instantiate(body, avatarPrefab.transform);
                bodyClone.name = "Body";
                avatar.body = bodyClone;
                // save as prefab
         
                // if path isn't exists - create it
                UnityEditor.PrefabUtility.SaveAsPrefabAsset(avatarPrefab, $"{path}{avatar.name}.prefab");
            }
            AddAvatars();
            // if prefab CharacterName_Character not exists in path - create it
            if(!System.IO.File.Exists($"{path}{source.name}.prefab"))
                UnityEditor.PrefabUtility.SaveAsPrefabAsset(this.gameObject, $"{path}{source.name}.prefab");
        }
        #endif
        public void OnValidate()
        {
            AddAvatars();
        }
        
        public AvatarType CurrentType
        {
            get => _currentType;
            set
            {
                _currentType = value;
                foreach (Avatar avatar in _avatarsDictionary.Values)
                {
                    avatar.gameObject.SetActive(avatar.type == value);
                }
            }
        }
        
        private void FindAvatar(AvatarCell cell)
        {
            if (cell.avatar != null)
            {
                _avatarsDictionary[cell.type] = cell.avatar;
            }
            
            // if child contains avatar component and avatar type == type 
            Avatar[] children = GetComponentsInChildren<Avatar>();
            foreach (Avatar child in children)
            {
                if (child.type == cell.type)
                {
                    cell.avatar = child;
                    _avatarsDictionary[cell.type] = child;
                    return;
                }
            }
        }
        
        private void AddAvatars()
        {
            var avatarTypes = Enum.GetValues(typeof(AvatarType));
            foreach (AvatarType avatarType in avatarTypes)
            {
                if (!_avatarsDictionary.ContainsKey(avatarType))
                {
                    bool hasFound = false;
                    for(int i = 0; i < _avatars.Count; i++)
                    {
                        if (_avatars[i].type == avatarType)
                        {
                            hasFound = true;
                            FindAvatar(_avatars[i]);
                            break;
                        }
                    }

                    if (!hasFound)
                    {
                        var cell = new AvatarCell(avatarType);
                        FindAvatar(cell);
                    }
                }
            }
            CurrentType = _currentType;
        }

        public void ChangeAvatar(in BodyAnchors anchors, in string characterName)
        {
            
        }
    }
}
