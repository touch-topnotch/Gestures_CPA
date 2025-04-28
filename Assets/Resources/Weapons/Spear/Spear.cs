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
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _endSpawnPoint;
    [SerializeField] private float _spawnDuration;
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
                transform.position = playerData.bodyAnchors.Body.position;
                transform.rotation = playerData.bodyAnchors.Body.rotation;
                break;
            case 8:
                _spearObject.transform.position = _spawnPoint.position;
                _spearObject.transform.rotation = Quaternion.LookRotation(Vector3.up);
                _spearObject.SetActive(true);
                StartCoroutine(SpawnSpear());
                break;
        }
    }
    
    private IEnumerator SpawnSpear()
    {
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(_spawnPoint.position, _endSpawnPoint.position);

        while (Time.time < startTime + _spawnDuration)
        {
            float distCovered = (Time.time - startTime) * journeyLength / _spawnDuration;
            float fracJourney = distCovered / journeyLength;
            _spearObject.transform.position = Vector3.Lerp(_spawnPoint.position, _endSpawnPoint.position, fracJourney);
            yield return null;
        }

        _spearObject.transform.position = _endSpawnPoint.position;
        _spearAura.gameObject.SetActive(true);
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
