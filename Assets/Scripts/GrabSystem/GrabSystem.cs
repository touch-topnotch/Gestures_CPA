using System;
using System.Collections;
using System.Collections.Generic;
using Gesture_Editor_SDK.ReadOnly;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class GrabSystem : MonoBehaviour
{
    public event Action OnGrabStart;
    public event Action OnGrabEnd;
    
    private PlayerData _playerData;
    [SerializeField] private Transform _grabObject;
    
    [Header("Grab Points")] 
    [SerializeField] private Transform _mainGrabPoint;
    [SerializeField] private float _mainGrabPointRadius;
    private bool _mainGrabbed;
    private Transform _mainGrabberTransform;

    
    [SerializeField] private Transform _secondaryGrabPoint;
    [SerializeField] private float _secondaryGrabPointRadius;
    private bool _secondaryGrabbed;


    [Header("Grab Gestures")] 
    [SerializeField] private string rightHandGrabGesture;
    [SerializeField] private string leftHandGrabGesture;

    
    public void SetPlayerData(PlayerData data)
    {
        _playerData = data;
    }

    private void Update()
    {
        HandleMainGrab();
        if (_mainGrabbed)
        {
            _grabObject.transform.rotation = _mainGrabberTransform.rotation * _mainGrabPoint.localRotation;
            _grabObject.transform.position = _mainGrabberTransform.position + (_grabObject.transform.position - _mainGrabPoint.position);
        }
    }

    private void HandleMainGrab()
    {
        if (!_mainGrabbed)
        {
            if (CheckHandGesture(_playerData.hands.rightHand.points[0], _mainGrabPoint, _mainGrabPointRadius, rightHandGrabGesture)) {}
            else if (CheckHandGesture(_playerData.hands.leftHand.points[0], _mainGrabPoint, _mainGrabPointRadius, leftHandGrabGesture)){}
            
        }
    }

    private bool CheckHandGesture(Transform hand, Transform grabPoint, float grabPointRadius, string gesture)
    {
        if (Vector3.Distance(hand.position, grabPoint.position) < grabPointRadius &&
            _playerData.recognizer.RecognizeFrame(new RecognitionProperties
                {
                    positionQuality = 0,
                    rotationQuality = 0.9f,
                    rootRotationQuality = 0,
                },
                _playerData.library.supportiveGestures[gesture], false))
        {
            _grabObject.rotation = hand.rotation;
            _grabObject.position += hand.position - grabPoint.position;
            _mainGrabberTransform = hand;

            if (!_mainGrabbed) OnGrabStart?.Invoke();
            _mainGrabbed = true;
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_mainGrabPoint.position, _mainGrabPointRadius);
    }
}
