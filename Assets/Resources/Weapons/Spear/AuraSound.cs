using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using Scripts.Systems;
using UnityEngine;
using Random = UnityEngine.Random;

public class AuraSound : PrefabSerializedMonoBehaviour
{
    [SerializeField] private AudioProcessor _audioProcessor;
   
    [Header("Sounds")]
    [SerializeField] private string[] _startSounds;
    [SerializeField] private string[] _startSoundsRandom;
    [SerializeField] private SerializableDictionary<string, float> _continuousSoundGroups;
    [SerializeField] private string[] _endSounds;
    [SerializeField] private string[] _endSoundsRandom;
    
    void OnEnable()
    {
        foreach (var sound in _startSounds)
            _audioProcessor.ActivateResource(sound);
        
        foreach (var sound in _startSoundsRandom)
            _audioProcessor.ActivateRandomResource(sound);
        
        foreach (var soundGroup in _continuousSoundGroups)
            StartCoroutine(ProduceRandomSounds(soundGroup.Key, soundGroup.Value));
    }
    
    private void OnDisable()
    {
        StopAllCoroutines();
        
        foreach (var sound in _endSounds)
            _audioProcessor.ActivateResource(sound);
        
        foreach (var sound in _endSoundsRandom)
            _audioProcessor.ActivateRandomResource(sound);
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
