
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Characters;
using Components;
using Scripts.Characters;
using Scripts.Design;
using Scripts.Events;
using Scripts.PlayerLogic;
using Scripts.Static;
using Scripts.Weapons;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Avatar = Scripts.PlayerLogic.Avatar;
public class CharacterCreatorWindow: OdinEditorWindow
{
    [MenuItem("Tools/CharacterCreator")]
    private static void OpenWindow()
    {
        GetWindow<CharacterCreatorWindow>().Show();
    }
    [Header("Character")]  [OnValueChanged("ChangeConfigName")]
    [BoxGroup("Properties")] public string characterName;
    [Tooltip("Model of character with 2 children: Head and Body with correctly constructed pivots")]
    [BoxGroup("Properties",true, true)]
    [PreviewField(100)]
    public GameObject characterModel;
   

    [BoxGroup("Properties")] public WeaponData[] weapons; 
    
    [OnValueChanged("CreateConfigFile")]
    [FoldoutGroup("Add Hand Appearance")]
    public bool configureHandAppearance;

    [FoldoutGroup("Add Hand Appearance")] [ShowIf("configureHandAppearance")]
    public string handConfigName;
    [FoldoutGroup("Add Hand Appearance")] [ShowIf("configureHandAppearance")] [InlineEditor()]
    public HandAppearance handAppearance;
    private void ChangeConfigName()
    {
        handConfigName = "HandAppearance_" + characterName;
    }
    
    private void CreateConfigFile()
    {
        if (configureHandAppearance)
        {
            var targetPath = "Assets/Resources/Characters/" + characterName + "/" + handConfigName + ".asset";
            if (handAppearance != null)
            {
                var charName = handAppearance.name.Split("_")[0];
                var wrongPath = "Assets/Resources/Characters/" + charName;
                
                // if file exists and name is different, find, rename and move to the right folder
                if (charName != characterName)
                {
                    Debug.Log("File with same name has found, renaming and moving to the right folder");
                    AssetDatabase.MoveAsset(wrongPath +"/"+ handAppearance.name + ".asset", targetPath);
                    Debug.Log("File has been moved to " + targetPath);
                    if (Directory.Exists(wrongPath) && Directory.GetFiles(wrongPath).Length < 2)
                    {
                       // delete other files and folder charNameт
                       
                          Directory.Delete("wrongPath");
                          Debug.Log("Folder " + wrongPath + " has been deleted");
                    }
                    
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                return;
            }
            
            // generate HandAppearance file
            string folderPath = "Assets/Resources/Characters/" + characterName + "/";
            
            if(!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            
            handAppearance = CreateInstance<HandAppearance>();
            AssetDatabase.CreateAsset(handAppearance, targetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            var asset = AssetDatabase.LoadAssetAtPath<HandAppearance>(targetPath);
            handAppearance = asset;
            Debug.Log("HandAppearance file has been created");
           
        }
    }

    [Button(ButtonSizes.Large), GUIColor("#F699CD")]
    [InfoBox("Убедитесь, что все файлы загружены в Telegram Resources")]
    [Tooltip(
        "Naming: S_ sounds, V_ vfx, M_ models, _S_ sequenced, _R_ random, _N_ neutral, ItemName, EffectType, Index (optional). Example: S_R_Guns_Shoot_1")]

    public void Bake()
    {
        Debug.Log("Bake started!");
        CheckComponents();
        GenerateCharacterData();
    }

    public void CheckComponents()
    {
        Debug.Log("Checking Resources");
        // has no russian letters, /, \, :, *, ?, ", <, >, |, #, %, ~, & and space
        if (string.IsNullOrEmpty(characterName) && characterName.Length < 3 && characterName.Length > 20)
            throw new UnityException("Wrong character name");
        
        // check telegram folder. Is any resources here
        // if(!Directory.Exists(telegramResources))
        //     throw new UnityException("Telegram resources folder is not found");
        
        // check character prefab
        if (characterModel == null)
            throw new UnityException("Character prefab is not found");
        
        // if character Prefab not contains Head and Body
        if (characterModel.transform.Find("Head") == null || characterModel.transform.Find("Body") == null)
            throw new UnityException("Character prefab should contains Head and Body");
        
        // check weapons
        // if (weapons.Length == 0)
        //     throw new UnityException("Weapons are not found");
        
        // if weapons has no name 
        for(int i = 0; i < weapons.Length; i++)
        {
            if (string.IsNullOrEmpty(weapons[i].weaponName))
                throw new UnityException("Weapon name is not found");
            if(weapons[i].addDesign && weapons[i].weaponDesign == null)
                throw new UnityException("Weapon design is not found");
            if(weapons[i].addCustomLogic && weapons[i].weaponLogic == null)
                throw new UnityException("Weapon logic is not found");
        }
        if(configureHandAppearance && handAppearance == null)
            throw new UnityException("Hand appearance is not found");
    }

    public void GenerateCharacterData()
    {

        string path = CustomPaths.CharacterNameFolder(characterName);
        CreateIfNotExist(path);
        // create object of CharacterData scripltable object file
        var characterData = CreateInstance<CharacterData>();
        
        characterData.characterName = characterName;
        var modelInstance = Instantiate(characterModel);
        
        characterData.avatars = GenerateAvatars(characterModel, characterName, path);
        characterData.weapons = GenerateWeapons(weapons, CustomPaths.Weapons);
        if(configureHandAppearance)
            characterData.handAppearance = handAppearance;
        
        AssetDatabase.CreateAsset(characterData, path + "/CharData_"+characterName+ ".asset");
        AssetDatabase.SaveAssets();
        
        
        var characterPoolAsset = AssetDatabase.LoadAssetAtPath<GameObject>(CustomPaths.CharacterManager);
        var characterPoolInstance =
            (PrefabUtility.InstantiatePrefab(characterPoolAsset) as GameObject);
        if (characterPoolInstance)
        {
            var characterPool = characterPoolInstance.GetComponent<CharacterPool>();
            if (characterPool)
            {
                var isReplaced = false;
                for (int i = 0; i < characterPool.characterConfigs.Count; i++)
                {
                    if (characterPool.characterConfigs[i]?.characterName == characterName)
                    {
                        isReplaced = true;
                        characterPool.characterConfigs[i] =
                            AssetDatabase.LoadAssetAtPath<CharacterData>(path + "/CharData_" + characterName +
                                                                         ".asset");
                        break;
                    }
                }

                if (!isReplaced)
                    characterPool.characterConfigs.Add(characterData);
            }

            PrefabUtility.SaveAsPrefabAssetAndConnect(characterPoolInstance,
                "Assets/Prefabs/Managers/CharacterController.prefab", InteractionMode.AutomatedAction);
        }
        DestroyImmediate(modelInstance);
        DestroyImmediate(characterPoolAsset);
        DestroyImmediate(characterPoolInstance);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    
        Debug.Log("Completed!");
    }

    private static void CreateIfNotExist(string path)
    {
        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }
    }
    public static Dictionary<string, GameObject> GenerateWeapons(WeaponData[] data, string path)
    {
        var d = new Dictionary<string, GameObject>();
        foreach (WeaponData weaponData in data)
        {
            GameObject weaponInstance = new GameObject();
            weaponInstance.name = weaponData.weaponName + "_Weapon";
            if (weaponData.addDesign)
                weaponInstance.AddComponent(weaponData.weaponDesign.GetClass());
            if (weaponData.addCustomLogic)
                weaponInstance.AddComponent(weaponData.weaponLogic.GetClass());
            weaponInstance.AddComponent<AudioProcessor>();
            weaponInstance.AddComponent<VFXProcessor>();
            CreateIfNotExist($"{path}/{weaponData.weaponName}");
            var o = PrefabUtility.SaveAsPrefabAsset(weaponInstance, $"{path}/{weaponData.weaponName}/{weaponData.weaponName}_Weapon.prefab");
            
            d.Add(weaponData.weaponName, o);
            DestroyImmediate(weaponInstance);
            AssetDatabase.Refresh();
        }

        return d;
    }
    public static GameObject GenerateAvatar(AvatarType avatarType, Transform head, Transform body, string path, string characterName)
    {
        if (avatarType == AvatarType.None)
            return null;
        // create new GameObject
        // add Avatar component
        // add Head and Body from source
        // save as prefab

        var avatarPrefab = new GameObject();
        

        avatarPrefab.name = $"{characterName}_{avatarType}";
        var avatar = avatarPrefab.AddComponent<Avatar>();
        
        var anchors = avatarPrefab.AddComponent<BodyAnchors>();
        
        avatar.Anchors = anchors;
        avatar.GetComponent<Avatar>().type = avatarType;

        var headClone = Instantiate(head, avatarPrefab.transform);
        headClone.name = "Head";
        avatar.Anchors.Head = headClone;
        headClone.gameObject.SetActive(avatarType != AvatarType.Local);

        var bodyClone = Instantiate(body, avatarPrefab.transform);
        bodyClone.name = "Body";
        avatar.Anchors.Body = bodyClone;
        var o = UnityEditor.PrefabUtility.SaveAsPrefabAsset(avatarPrefab, $"{path}/{avatar.name}.prefab");
        DestroyImmediate(avatarPrefab);
        AssetDatabase.Refresh();
        return o;
    }
    public static Dictionary<AvatarType, GameObject> GenerateAvatars(GameObject model, string characterName, string path)
    {
        var v = Enum.GetValues(typeof(AvatarType));
        
        Transform head = model.transform.Find("Head");
        Transform body = model.transform.Find("Body");
        
        if (head == null || body == null)
        {
            throw new UnityException("Body parts are null");
        }
        
        var d = new Dictionary<AvatarType, GameObject>();
        foreach (AvatarType avatarType in v)
        {
            if(avatarType == AvatarType.None)
                continue;
            d.Add(avatarType, GenerateAvatar(avatarType, head, body, path,characterName));
        }

        return d;
    }
}

[Serializable]
public class WeaponData
{
    [BoxGroup("Properties")] public WeaponClass weaponClass;
    [BoxGroup("Properties")] public string weaponName;
    
    
    [ToggleGroup("addDesign", "Add Design")]
    public bool addDesign;

    [ToggleGroup("addDesign", "Add Design")][OnValueChanged("CheckDesign")]
    public MonoScript weaponDesign;
    
    [ToggleGroup("addCustomLogic", "Add Custom Weapon Logic")]
    public bool addCustomLogic;

    [ToggleGroup("addCustomLogic", "Add Custom Weapon Logic")]
    public MonoScript weaponLogic;

    
    private void CheckDesign()
    {
        weaponDesign = TryAddComponent<WeaponDesign>(weaponDesign);
    }
    private void CheckLogic()
    {
        weaponLogic = TryAddComponent<WeaponDesign>(weaponLogic);
    }
    
    private MonoScript TryAddComponent<T>(MonoScript o)
    where T: Component
    {
        if (!o.GetClass().IsSubclassOf(typeof(T)))
        {
            
            Debug.LogWarning("Wrong script type! Should be type of " + typeof(T));
            o = null;
        }
        return o;
    }
    
}
#endif