using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SplineMesh;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts
{
    public class SplineFollow : MonoBehaviour
    {
        [SerializeField] private Spline _spline;
        [SerializeField] private GameObject _follower;
        [SerializeField] private Ease _movementEase;
        [SerializeField] private Ease _movementEaseReversed;
        [SerializeField] private bool _followSplineRotation;
        [SerializeField] private float DurationInSecond;

        public bool IsMoving
        {
            get => _isMoving;
            private set => _isMoving = value;
        }

        public event Action OnCompleted; 
        
        private bool _isMoving;
        

        private Tweener _tweener;

        public void StartMovement(bool reversed = false, bool interrupt = false)
        {
            if (IsMoving)
            {
                if (interrupt) _tweener?.Kill();
                else return;
            }

            IsMoving = true;

            if (reversed)
                _tweener = DOVirtual.Float(_spline.nodes.Count - 1, 0, DurationInSecond, PlaceFollower).SetEase(_movementEaseReversed);
            else
                _tweener = DOVirtual.Float(0, _spline.nodes.Count - 1, DurationInSecond, PlaceFollower).SetEase(_movementEase);
            
            _tweener.OnComplete(MovementCompleted);
        }

        private void MovementCompleted()
        {
            IsMoving = false;
            Debug.Log("Elevator Completed");
            OnCompleted?.Invoke();
        }
        
        private void PlaceFollower(float value)
        {
            if (_follower != null) {
                CurveSample sample = _spline.GetSample(value);
                _follower.transform.localPosition = sample.location;
                if (_followSplineRotation) _follower.transform.localRotation = sample.Rotation;
            }
        }
    }
}
