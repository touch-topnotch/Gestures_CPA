using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using Scripts.Design;
using Scripts.Gestures;
using Scripts.HandsLogic;
using UnityEngine;
using UnityEngine.Serialization;

public class Spear : WeaponDesign
{
    [SerializeField] private LayerMask _floorMask;

    [SerializeField] private AudioProcessor weaponAudioProcessor;
    
    [Header("Spear Settings")]
    [SerializeField] private Transform _startSpawnPoint;
    [SerializeField] private float _spawnDuration;
    [SerializeField] private Animation _spawnAnimation;
    [SerializeField] private GameObject _spearObject;
    
    [Header("Aura")]
    [SerializeField] private GameObject _spearAura;
    
    [Header("VFX Objects")]
    [SerializeField] private GameObject _portalVFX;
    private Vector3 _portalSpawnLocalPos;
    
    private bool _shouldPortalFollowHandPosStop;
    private bool _shouldPortalFollowHandRotZStop;
    private bool _shouldPortalFollowHandRotXStop;
    

    private void Awake()
    {
        _spearObject.SetActive(false);
        _portalSpawnLocalPos = _portalVFX.transform.localPosition;
    }

    public override void OnFrameRecognized(string frameName)
    {
        var frameId = GestureMapper.IndexOfName(frameName);
        Debug.Log("Design FrameRecognized " + frameId);
        
        audioProcessor.ActivateResource("Frame_" + frameId);
        
        
        switch (frameId)
        {
            case 1: // 0
                transform.position = playerData.anchors.Root.transform.position;
                transform.rotation = playerData.anchors.Body.transform.rotation;
                
                RaycastHit hit;
                if (Physics.Raycast(_startSpawnPoint.position, Vector3.down, out hit, 10f, _floorMask))
                {
                    transform.position = hit.point;
                }
                break;
            case 3:
                _portalVFX.transform.localPosition = _portalSpawnLocalPos;
                _portalVFX.SetActive(true);
                _shouldPortalFollowHandPosStop = false;
                _shouldPortalFollowHandRotZStop = false;
                _shouldPortalFollowHandRotXStop = false;

                StartCoroutine(PortalFollowHandPos());
                StartCoroutine(PortalFollowHandRot());
                break;
            case 4:
                _shouldPortalFollowHandRotXStop = true;
                break;
            case 5: 
                _shouldPortalFollowHandRotZStop = true;
                break;
            case 7:
                _shouldPortalFollowHandPosStop = true;
                break;
            case 8:
                _spearObject.transform.rotation = Quaternion.LookRotation(Vector3.up);
                
                StartCoroutine(SpawnSpear());
                break;
        }
    }
    
    private IEnumerator PortalFollowHandPos()
    {
        var targetPos = playerData.hands.rightHand.points[0].position.y + 0.1f;
        var portalTransform = _portalVFX.transform;

        while (!_shouldPortalFollowHandPosStop || Vector3.Distance(portalTransform.position,
                   new Vector3(portalTransform.position.x, targetPos, portalTransform.position.z)) > 0.01f)
        {
            if (!_shouldPortalFollowHandPosStop) 
            {
                targetPos = playerData.hands.rightHand.points[0].position.y + 0.1f;
            }

            var position = portalTransform.position;
            position = Vector3.Lerp(position, new Vector3(position.x, targetPos, position.z), 2f * Time.deltaTime);
            portalTransform.position = position;

            yield return null;
        }
    }
    
    private IEnumerator PortalFollowHandRot()
    {
        float targetRotZ = playerData.hands.rightHand.points[0].rotation.eulerAngles.z;
        float targetRotX = playerData.hands.rightHand.points[0].rotation.eulerAngles.x;

        while (!_shouldPortalFollowHandRotZStop || Vector3.Distance(_portalVFX.transform.eulerAngles, new Vector3(targetRotX,0, targetRotZ)) > 0.1f)
        {
            if (!_shouldPortalFollowHandRotZStop) targetRotZ = playerData.hands.rightHand.points[0].rotation.eulerAngles.z;
            else targetRotZ = -180f;
            
            if (!_shouldPortalFollowHandRotXStop) targetRotX = playerData.hands.rightHand.points[0].rotation.eulerAngles.x;
            else targetRotX = 0f;
            
            
            Quaternion targetQuaternion = Quaternion.Euler(targetRotX, 0, targetRotZ);
            _portalVFX.transform.rotation = Quaternion.Slerp(_portalVFX.transform.rotation, targetQuaternion, 8f * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator SpawnSpear()
    {
        yield return new WaitForSeconds(1 - _spawnDuration);
        _spearObject.SetActive(true);
        _spawnAnimation.Play();

        yield return new WaitForSeconds(_spawnDuration);
        
        _spearAura.SetActive(true);
    }

    public override void OnGestureDetected()
    {
        Debug.Log("GestureDetected");
    }

    public override void OnHitHolding()
    {
      //  Debug.Log("HitHolding");
    }

    public override void OnHitCalled()
    {
        //Debug.Log("HitCalled");
        weaponAudioProcessor.ActivateRandomResource("Swing");
    }

    public override void OnHitImpact(string affected)
    {
        weaponAudioProcessor.ActivateRandomResource("Hit_" + affected);
    }

    public override void OnAbilityReleased()
    {
        Debug.Log("AbilityReleased");

    }
}
