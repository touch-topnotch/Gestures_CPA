
#if UNITY_EDITOR
using System;
using System.IO;
using Components;
using Scripts.Characters;
using Scripts.Design;
using Scripts.PlayerLogic;
using Scripts.Weapons;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Unity.VisualScripting;
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


    [BoxGroup("Properties")] [FolderPath]
    public string telegramResources;
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
        Debug.Log("Test");
        handConfigName = "Hand_Appearance_"+characterName;
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
                    if (Directory.GetFiles(wrongPath).Length < 2)
                    {
                       // delete other files and folder charName
                       
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
            
            handAppearance = ScriptableObject.CreateInstance<HandAppearance>();
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
        GenerateCharacter();
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

    public void GenerateCharacter()
    {
        GameObject characterInstance = new();
        var character = characterInstance.AddComponent<Character>();
        character.AddComponent<HandAppearanceProcessor>();
        character.handAppearance = character.GetComponent<HandAppearanceProcessor>();
        if(configureHandAppearance)
            character.handAppearance.handAppearanceConfig = handAppearance;
        var modelInstance = Instantiate(characterModel);
        var prefChar =  GeneratePrefabs(character, modelInstance, characterName);
        
        var prefAsset = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Managers/CharacterController.prefab");
        
        var characterPoolInstance =
            PrefabUtility.InstantiatePrefab(prefAsset) as GameObject;
        
        if (!characterPoolInstance.transform.Find(characterName))
        {
            var charI = Instantiate(prefChar, characterPoolInstance.transform);
            charI.name = characterName;
            characterPoolInstance.GetComponent<CharacterPool>().AddCharacter(charI.GetComponent<Character>());
        }

        PrefabUtility.SaveAsPrefabAssetAndConnect(characterPoolInstance,
            "Assets/Prefabs/Managers/CharacterController.prefab", InteractionMode.AutomatedAction);
       
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        DestroyImmediate(modelInstance);
        DestroyImmediate(characterInstance);
        DestroyImmediate(characterPoolInstance);
        DestroyImmediate(prefChar);
        DestroyImmediate(character);
        Debug.Log("Completed!");
    }

    public static GameObject GeneratePrefabs(Character character, GameObject characterPref, string characterName)
    {
        // if source name includes _Character - find children with name Body, Head
        // then generate avatars by path Resources/Avatars/CharacterName
        // each avatar must have Avatar component
        // each avatar must have Head and Body fromm source
        // avatars should be saved by path Resources/Avatars/CharacterName/CharacterName_AvatarType
        Transform head = characterPref.transform.Find("Head");
        Transform body = characterPref.transform.Find("Body");
        if (head == null || body == null)
        {
            throw new UnityException("Body parts are null");
        }
        string path = $"Assets/Resources/Characters/{characterName}/";

        if (!System.IO.Directory.Exists(path))
        {
            System.IO.Directory.CreateDirectory(path);
        }

        foreach (var avatarType in Enum.GetNames(typeof(AvatarType)))
        {
            if (avatarType == "None")
                continue;
            // create new GameObject
            // add Avatar component
            // add Head and Body from source
            // save as prefab

            var temporaryObject = new GameObject();
            var avatarPrefab = Instantiate(temporaryObject, character.transform);

            avatarPrefab.name = $"{characterName}_{avatarType}";
            var avatar = avatarPrefab.AddComponent<Avatar>();
            var anchors = avatarPrefab.AddComponent<BodyAnchors>();
            avatar.Anchors = anchors;
            avatar.GetComponent<Avatar>().type = (AvatarType)Enum.Parse(typeof(AvatarType), avatarType);

            var headClone = Instantiate(head, avatarPrefab.transform);
            headClone.name = "Head";
            avatar.Anchors.Head = headClone;
            headClone.gameObject.SetActive(avatarType != "Local");

            var bodyClone = Instantiate(body, avatarPrefab.transform);
            bodyClone.name = "Body";
            avatar.Anchors.Body = bodyClone;

            UnityEditor.PrefabUtility.SaveAsPrefabAsset(avatarPrefab, $"{path}{avatar.name}.prefab");
            DestroyImmediate(temporaryObject);
        }

        character.FindAvatars();
        
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(character.gameObject, $"{path}{characterName}.prefab");
        AssetDatabase.Refresh();
        return AssetDatabase.LoadAssetAtPath<GameObject>($"{path}{characterName}.prefab");
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