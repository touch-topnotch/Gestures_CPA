using System.Collections.Generic;
using System.Linq;
using Characters;
using Scrips.Components;
using Scripts.Events;
using Scripts.Gesture_Editor_SDK.Realtime;
using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.Libraries;
using Scripts.PlayerLogic;
using Scripts.Static;
using Scripts.Static.Definitions;
using Scripts.Systems;
using Scripts.Weapons;
using Sirenix.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Scripts.Abilities
{
    public class AbilityController : PlayerComponent
    {
        // TODO: change types of access
        // TODO: сервер выбирает арсенал из доступных оружий для игрока. Ему подается запрос - AbilitiesInitializedServerRpc(string[] abilitynames) - он возвращает содержимое арсенала
        // где связать сервер? GameController? Ну да, ура, ты до него дошел
        public Inventory inventory;
        public AbilitiesLibrary abilitiesLib;
        public GesturesLibrary gesturesLib;
        public UnityEvent OnWeaponsInitialized = new UnityEvent();

        private FrameRecognized OnAbilityFrameRecognized;
        private GestureRecognized OnGestureRecognized;
        
        private Recognizer _recognizer;

        public void Initialize()
        {
            abilitiesLib = new AbilitiesLibrary();
            gesturesLib = new GesturesLibrary();
            inventory = new Inventory();
            OnGestureRecognized = new GestureRecognized();
            OnAbilityFrameRecognized = new FrameRecognized();
       

            // TODO: че тут написано вообще?
            
            if(Player.modesWithGestureRecognition.Contains(inherited.playerMode))
            {
                OnAbilityFrameRecognized.AddListener((e) =>
                {
                 //   abilitiesLib[AbilityType.Character][GestureMapper.PrefixOfName(e)].OnFrameRecognized(e);
                });
                OnGestureRecognized.AddListener((e) =>
                {
                 //   gesturesLib.characterGestures[e].AllFramesDetected(RecognizeWithAllGestures);
                });
            }
        }

        public void CreateRecognizer(RecognitionPropertiesConfig config)
        {
            _recognizer = new Recognizer(config);
        }

        public void AddCharacterToInventory(string character)
        {
            if (abilitiesLib.characterAbilities.ContainsKey(character) && abilitiesLib.characterAbilities[character] != null)
            {
                inventory.characterAbilities.AddReplace(abilitiesLib.characterAbilities[character]);
                //Debug.Log("Inventory " + character+" abilities: " + Debugger.dictionaryToString(abilitiesLib.characterAbilities[character], false, true));
            }
          
        }
        public void UseCharacterAbilities()
        {
            Debug.Log("Start to use next inventory abilities "+Debugger.dictionaryToString(inventory.characterAbilities, false, true));
            StartCoroutine(_recognizer.RecognizeDynamicGesture(inventory.characterAbilities.ToGestureDict(),
                (e) =>
                {
                    inventory.characterAbilities[e].OnGestureCasted();
                }
                , (e) =>
                {
                    inventory.characterAbilities[GestureMapper.PrefixOfName(e)]?.OnFrameRecognized(e);
                }));
        }
        
        
        // Simulate frame - is a specific function, which needs to simulate Hands movement on other (enemy) client device.
        public void SimulateFrame(PlayerHands hands, string name)
        {
            if (gesturesLib.TryGetDynamicGesture(name, out var gesture))
            {
                if (gesture.TryGetFrameData(name, out var frame))
                {
                    Debug.Log("Move hands");
                    hands.MoveHands(frame.ParentedFrame(inherited.data.anchors.Body), 4,
                        () => { Debug.Log("Frame Simulated!"); },
                        true);
                }
            }
        }

        protected override bool shouldAddMissingComponents => false;

        
        public void SpawnWeapons(List<CharacterData> characterConfigs, Transform parent)
         {
            
            foreach (var characterData in characterConfigs)
            {
                var weapons = new Arsenal();
                foreach (var weaponStruct in characterData.weapons)
                {
                    var key = weaponStruct.Key;
                    var prefab = weaponStruct.Value;
                    if (weapons.ContainsKey(key))
                    {
                        Debug.Log($"Was found a weapon with the same key {key}");
                        continue;
                    }

                    if (!prefab)
                    {
                        Debug.Log($"Prefab {key} is null");
                        continue;
                    }

                    if (!prefab.GetComponent<Weapon>())
                    {
                        Debug.Log($"Prefab {key} doesn't contain Weapon script");
                        continue;
                    }

                    if (!gesturesLib.characterGestures.ContainsKey(key))
                    {
                        Debug.Log($"Gesture library doesn't contain {key}");
                        continue;
                    }

                    var spawnedWeapon = Instantiate(prefab).GetComponent<Weapon>();
                    spawnedWeapon.NetworkObject.Spawn();
                    if (!spawnedWeapon.NetworkObject.TrySetParent(parent))
                    {
                        Debug.Log($"Can't set parent for {key}");
                        continue;
                    }
                    spawnedWeapon.Initialize(inherited.data, gesturesLib.characterGestures[key]);
                    weapons.AddReplace(spawnedWeapon.abilityName, spawnedWeapon);
                    
                }
                abilitiesLib.characterAbilities.AddReplace(characterData.characterName, weapons);
            }

            var log = "Weapons (spawn) initialized: ";

            foreach (var VARIABLE in abilitiesLib.characterAbilities)
            {
                log += VARIABLE.Key + " contains " + Debugger.dictionaryToString(VARIABLE.Value, false, false) + "; ";
            }
            Debug.Log(log);
            OnWeaponsInitialized?.Invoke();
         }

        public void SetSpawnedWeapons(Dictionary<CharacterType, ulong[]> dictionary, Weapon[] spawned)
        {
            foreach (var characterWeapons in dictionary)
            {
                var weapons = new Arsenal();
                foreach (var weapon_ulong in characterWeapons.Value)
                {
                    Weapon w = null;
                    foreach (var s in spawned)
                    {
                        if (s.NetworkObjectId == weapon_ulong)
                        {
                            w = s;
                        }
                    }
                    if(w == null)
                    {
                        Debug.Log($"Weapon with id {weapon_ulong} was not found");
                        continue;
                    }

                    if(!gesturesLib.characterGestures.ContainsKey(w.abilityName))
                    {
                        Debug.Log($"Gesture library doesn't contain {w.abilityName}");
                        continue;
                    }
                    w.Initialize(inherited.data, gesturesLib.characterGestures[w.abilityName]);
                    weapons.Add(w.name.Split('_')[0], w);
                 
                }
                abilitiesLib.characterAbilities.AddReplace(characterWeapons.Key.ToString(), weapons);
            }
            var log = "Weapons (set) initialized: ";
            foreach (var VARIABLE in abilitiesLib.characterAbilities)
            {
                log += VARIABLE.Key + " contains " + Debugger.dictionaryToString(VARIABLE.Value, false, false) + "; ";
            }
            Debug.Log(log);
            OnWeaponsInitialized?.Invoke();
        }
    }
}