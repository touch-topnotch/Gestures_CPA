using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpearAura : MonoBehaviour
{
    [SerializeField] private AudioProcessor _audioProcessor;
    [SerializeField] private float delayHorseSound;
    [SerializeField] private float delayCrowdSound;
    [SerializeField] private float delayAnvilSound;
    
    
    void OnEnable()
    {
        _audioProcessor.ActivateResource("Start_Aura");
        _audioProcessor.ActivateRandomResource("Crowd_Aura");
        StartCoroutine(ProduceRandomSounds("Horse_Aura", delayHorseSound));
        StartCoroutine(ProduceRandomSounds("Crowd_Aura", delayCrowdSound));
        StartCoroutine(ProduceRandomSounds("Anvil_Aura", delayAnvilSound));
    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
        _audioProcessor.ActivateResource("End_Aura");
    }

    private IEnumerator ProduceRandomSounds(string name, float averageDelay)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(averageDelay * 0.8f, averageDelay * 1.2f));
            _audioProcessor.ActivateRandomResource(name);
        }
    }
}
