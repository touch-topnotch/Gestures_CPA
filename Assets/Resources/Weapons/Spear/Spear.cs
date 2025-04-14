using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using Scripts.Design;
using Scripts.Gestures;
using Scripts.HandsLogic;
using UnityEngine;

public class Spear : WeaponDesign
{
    [Header("Spear Settings")]
    [SerializeField] private Transform _endSpawnPoint;
    [SerializeField] private float _spawnDuration;
    [SerializeField] private Animation _spawnAnimation;
    [SerializeField] private GameObject _spearObject;
    [SerializeField] private SpearAura _spearAura;

    private void Awake()
    {
        _spearObject.SetActive(false);
    }

    public override void OnFrameRecognized(string frameName)
    {
        var frameId = GestureMapper.IndexOfName(frameName);
        Debug.Log("Design FrameRecognized " + frameId);
        
        audioProcessor.ActivateResource("Frame_" + frameId);
        
        
        switch (frameId)
        {
            case 0:
                transform.position = _endSpawnPoint.transform.position;
                transform.rotation = playerData.playerTransform.rotation;
                break;
            case 7:
                vfxProcessor.ActivateResource("Frame_" + frameId, (GameObject o)=>
                {
                    o.SetActive(true);
                });
                break;
            case 8:
                _spearObject.transform.rotation = Quaternion.LookRotation(Vector3.up);
                
                StartCoroutine(SpawnSpear());
                break;
        }
    }

    private IEnumerator SpawnSpear()
    {
        yield return new WaitForSeconds(1 - _spawnDuration);
        _spearObject.SetActive(true);
        _spawnAnimation.Play();

        yield return new WaitForSeconds(_spawnDuration);
        
        _spearAura.gameObject.SetActive(true);
    }

    public override void OnGestureDetected()
    {
        Debug.Log("GestureDetected");
    }

    public override void OnHitHolding()
    {
        Debug.Log("HitHolding");
    }

    public override void OnHitCalled()
    {
        //Debug.Log("HitCalled");
        audioProcessor.ActivateRandomResource("Swing");
    }

    public override void OnHitImpact(string affected)
    {
        audioProcessor.ActivateRandomResource("Hit_" + affected);
    }

    public override void OnAbilityReleased()
    {
        Debug.Log("AbilityReleased");

    }
}
