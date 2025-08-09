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
        private CharacterReflection _characterReflection;
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
            _characterChanger = new CharacterChanger(0);
            _playerData = PlayerData.local;

            _waterFallMaterial = _waterFallObject.GetComponent<MeshRenderer>().material;
            _waterReflectionDistortionID = Shader.PropertyToID(WaterReflectionDistortion);
            _waterStartReflectionDistortion = _waterFallMaterial.GetFloat(_waterReflectionDistortionID);

        }
        
        private void Update()
        {
            ///////////////////////////////////////////
            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(UpdateCharacterReflection(_characterChanger.GetPreviousCharacter()));

            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                StartCoroutine(UpdateCharacterReflection(_characterChanger.GetNextCharacter()));
            }
            ///////////////////////////////////////////

        }

        private void StartInteraction()
        { 
            _playerData.rig.headInteraction.onHeadInteraction.AddListener(SwitchCharacterInput());

            StartCoroutine(UpdateCharacterReflection(_characterChanger.GetCurrentCharacter()));

        }
        private void EndInteraction()
        {
            PlayerData.local.rig.headInteraction.onHeadInteraction.RemoveListener(SwitchCharacterInput());
            Destroy(_characterReflection.gameObject);
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
                StartCoroutine(UpdateCharacterReflection(_characterChanger.GetNextCharacter()));
            else
                StartCoroutine(UpdateCharacterReflection(_characterChanger.GetPreviousCharacter()));
        }

        private IEnumerator UpdateCharacterReflection(GameObject character)
        {
            DOVirtual.Float(_waterFallMaterial.GetFloat(_waterReflectionDistortionID), _maxRefraction, _changeDuration,
                v => _waterFallMaterial.SetFloat(_waterReflectionDistortionID, v)).SetEase(Ease.InCubic);
            _canInteract = false;
            
            yield return new WaitForSeconds(_changeDuration);
            if (_characterReflection)
            {
                Destroy(_characterReflection.gameObject);
            }
            _characterReflection = Instantiate(character).AddComponent<CharacterReflection>();
            
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
    }
}
