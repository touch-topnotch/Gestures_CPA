using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace Scripts
{
    public class PokeButtonVisualFollow : MonoBehaviour
    {
        [SerializeField] private PokeInteractButton _interactButton;
        [SerializeField] private XRBaseInteractable _interactable;
        [SerializeField] private Vector3 _localAxis;
        [SerializeField] private float _followAngleTreshold = 45f;
        [SerializeField] private float _resetSpeedLerp = 1;
        
        private bool _isFollowing;
        private bool _freeze;

        private Vector3 _initialLocalPos;
        private Vector3 _offset;
        private Transform _pokeAttachTransform;

        private void Start()
        {
            _initialLocalPos = transform.localPosition;
        }

        private void OnEnable()
        {
            _interactable.hoverEntered.AddListener(Follow);
            _interactable.selectEntered.AddListener(Freeze);
            _interactButton.OnReset.AddListener(Reset);
        }

        private void Reset()
        {
            _isFollowing = false;
            _freeze = false;
        }
        
        private void Freeze(BaseInteractionEventArgs hover)
        {
            if (hover.interactorObject is XRPokeInteractor)
            {
                _freeze = true;
            }
        }

        private void Follow(HoverEnterEventArgs hover)
        {
            if (_freeze) return;
            
            if (hover.interactorObject is XRPokeInteractor)
            {
                XRPokeInteractor interactor = (XRPokeInteractor) hover.interactorObject;
                
                _pokeAttachTransform = interactor.attachTransform;
                _offset = transform.position - _pokeAttachTransform.position;

                float pokeAngle = Vector3.Angle(_offset, transform.TransformDirection(_localAxis));
                if (pokeAngle <= _followAngleTreshold)
                {
                    _isFollowing = true;
                    _freeze = false;
                }
            }
        }

        private void Update()
        {
            if (_freeze) return;
            
            if (_isFollowing)
            {
                Vector3 localTargetPos = transform.InverseTransformPoint(_pokeAttachTransform.position + _offset);
                Vector3 constrainedLocalTargetPos = Vector3.Project(localTargetPos, _localAxis);
                
                transform.position = transform.TransformPoint(constrainedLocalTargetPos);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, _initialLocalPos, Time.deltaTime * _resetSpeedLerp);
            }
        }

        private void OnDisable()
        {
            _interactable.hoverEntered.RemoveListener(Follow);
            _interactable.selectEntered.RemoveListener(Freeze);
            _interactButton.OnReset.RemoveListener(Reset);

        }
    }
}
