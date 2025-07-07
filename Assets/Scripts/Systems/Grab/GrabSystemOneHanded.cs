using System;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using Scripts.Systems.Grab;
using UnityEngine;



internal class GrabSystemOneHanded : GrabSystem
{
    protected override void HandleGrab()
    {
        if (!_mainGrabPoint.IsGrabbed)
        {
            if (CheckHandGrab(rightHandGrabber, _mainGrabPoint, rightHandGrabGesture) ||
                CheckHandGrab(leftHandGrabber, _mainGrabPoint, leftHandGrabGesture))
            {
                if (!_mainGrabPoint.IsGrabbed) OnGrabStarted();
                if (_mainGrabPoint.UnGrabCoroutine != null) StopCoroutine(_mainGrabPoint.UnGrabCoroutine);
                _mainGrabPoint.IsGrabbed = true;
            }
        }
        else
        {
            if (!RecognizeFrame(_mainGrabPoint.GrabGesture))
            {
                if (!_mainGrabPoint.IsUnGrabbing && _mainGrabPoint.IsGrabbed)
                    _mainGrabPoint.UnGrabCoroutine = StartCoroutine(UnGrab(_mainGrabPoint, _mainGrabPoint.IsGrabbed));
            }
        }
    }

    protected override void SetGrabObjectTransform()
    {
        if (_mainGrabPoint.IsGrabbed)
        {
            SetGrabObjectTransformOneHanded(_mainGrabPoint);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_mainGrabPoint.GrabPointTransform.position, _mainGrabPoint.GrabPointRadius);
        Gizmos.DrawRay(_mainGrabPoint.GrabPointTransform.position, _mainGrabPoint.GrabPointTransform.right);
    }

    //protected override bool shouldAddMissingComponents { get; }
}