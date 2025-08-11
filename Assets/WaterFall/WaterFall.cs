using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.Players;
using Scripts.Static.Definitions;
using Scripts.Systems;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts
{
    public class WaterFall : MonoBehaviour
    {
        private CharacterChanger _characterChanger;
        private List<CharacterReflection> _characterReflections = new List<CharacterReflection>();
        private PlayerData _playerData;

        private bool _canInteract;

        [SerializeField] private GameObject _waterFallObject; 
        private Material _waterFallMaterial;

        [SerializeField] private float _changeDuration;
        [SerializeField] private float _maxRefraction = 1;
        private const string WaterReflectionDistortion = "_ReflectionRefraction";
        private int _waterReflectionDistortionID;
        private float _waterStartReflectionDistortion;
        
        
        
        
        private void Start()
        {
            _playerData = PlayerData.local;

            SpawnReflections();
            
            _waterFallMaterial = _waterFallObject.GetComponent<MeshRenderer>().material;
            _waterReflectionDistortionID = Shader.PropertyToID(WaterReflectionDistortion);
            _waterStartReflectionDistortion = _waterFallMaterial.GetFloat(_waterReflectionDistortionID);

        }
        
        private void Update()
        {
            ///////////////////////////////////////////
            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(UpdateCharacterReflection(_characterChanger.SelectPreviousCharacter()));

            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                StartCoroutine(UpdateCharacterReflection(_characterChanger.SelectNextCharacter()));
            }
            ///////////////////////////////////////////

        }

        private void StartInteraction()
        {
           

            _characterChanger ??= new CharacterChanger(_playerData.characterController.currentCharacter.name);
            
            _playerData.rig.headInteraction.onHeadInteraction.AddListener(SwitchCharacterInput());

            StartCoroutine(UpdateCharacterReflection(_characterChanger.SelectCurrentCharacter()));

        }
        private void EndInteraction()
        {
            PlayerData.local.rig.headInteraction.onHeadInteraction.RemoveListener(SwitchCharacterInput());
            DisableReflection();
        }

        private UnityAction<HeadInteractionType> SwitchCharacterInput()
        {
            return (headInteractionType) =>
            {
                if (!_canInteract) return;
                
                switch (headInteractionType)
                {
                    case HeadInteractionType.Left:
                        CharacterSwitch(false);
                        break;
                    case HeadInteractionType.Right:
                        CharacterSwitch(true);
                        break;
                }
            };
        }
        
        private void CharacterSwitch(bool isRight)
        {
            if (isRight)
                StartCoroutine(UpdateCharacterReflection(_characterChanger.SelectNextCharacter()));
            else
                StartCoroutine(UpdateCharacterReflection(_characterChanger.SelectPreviousCharacter()));
        }

        private IEnumerator UpdateCharacterReflection(GameObject character)
        {
            DOVirtual.Float(_waterFallMaterial.GetFloat(_waterReflectionDistortionID), _maxRefraction, _changeDuration,
                v => _waterFallMaterial.SetFloat(_waterReflectionDistortionID, v)).SetEase(Ease.InCubic);
            _canInteract = false;
            
            yield return new WaitForSeconds(_changeDuration);
            UpdateReflection();
            _characterChanger.SetCharacter();
            
            DOVirtual.Float(_waterFallMaterial.GetFloat(_waterReflectionDistortionID), _waterStartReflectionDistortion, _changeDuration,
                v => _waterFallMaterial.SetFloat(_waterReflectionDistortionID, v)).SetEase(Ease.OutCubic);
            
            yield return new WaitForSeconds(_changeDuration);
            _canInteract = true;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                StartInteraction();
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                EndInteraction();
            }
        }

        private void SpawnReflections()
        {
            foreach (var characterData in _playerData.characterController.characterConfigs)
            {
                var characterReflection = Instantiate(characterData.avatars[AvatarType.Enemy], transform).AddComponent<CharacterReflection>();
                _characterReflections.Add(characterReflection);
                characterReflection.gameObject.SetActive(false);
            }
        }

        private void UpdateReflection()
        {
            for (var i = 0; i < _characterReflections.Count; i++)
            {
                _characterReflections[i].gameObject.SetActive(i == _characterChanger.CurrentCharacterIndex);
            }
        }

        private void DisableReflection()
        {
            foreach (var reflection in _characterReflections)
            {
                reflection.gameObject.SetActive(false);
            }
        }
    }
}
