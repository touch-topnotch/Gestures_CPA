using System.Collections.Generic;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using Scripts.Systems;
using UnityEngine;

public class TestLib : MonoBehaviour
{
    [SerializeField] private Player _player;

    private Dictionary<string, DynamicGesture> _characterGestures;
    private Dictionary<string, GestureFrame> _supportiveGestures;
    private Dictionary<string, GestureFrame> _systemGestures;


    private void Awake()
    {
        _player.gestureCombiner.library.onLibraryInitialized += Library_OnLibraryInitialized;
    }

    private void Library_OnLibraryInitialized()
    {
        Debug.Log("I get this gestures");
        Debug.Log("character: " +
                  Debugger.dictionaryToString(_player.gestureCombiner.library.characterGestures, false, true));
        _characterGestures = _player.gestureCombiner.library.characterGestures;

        Debug.Log("supportive: " +
                  Debugger.dictionaryToString(_player.gestureCombiner.library.supportiveGestures, false, true));
        _supportiveGestures = _player.gestureCombiner.library.supportiveGestures;

        Debug.Log("system: " +
                  Debugger.dictionaryToString(_player.gestureCombiner.library.systemGestures, false, true));
        _systemGestures = _player.gestureCombiner.library.systemGestures;
    }

    void Start()
    {
        
    }

    public Dictionary<string, DynamicGesture> GetPlayerCharacterGestures()
    {
        return _player.gestureCombiner.library.characterGestures;
    }

    public Dictionary<string, GestureFrame> GetPlayerSupportiveGestures()
    {
        return _player.gestureCombiner.library.supportiveGestures;
    }

    public Dictionary<string, GestureFrame> GetPlayerSystemGestures()
    {
        return _player.gestureCombiner.library.systemGestures;
    }
}
