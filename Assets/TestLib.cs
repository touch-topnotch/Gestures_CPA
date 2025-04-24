using Scripts.PlayerLogic;
using Scripts.Systems;
using UnityEngine;

public class TestLib : MonoBehaviour
{
    [SerializeField] private Player _player;
    void Start()
    {
        _player.gestureCombiner.library.onLibraryInitialized += () =>
        {
            Debug.Log("I get this gestures");
            Debug.Log("character: " + Debugger.dictionaryToString(_player.gestureCombiner.library.characterGestures, false, true));
            Debug.Log("supportive: "+ Debugger.dictionaryToString(_player.gestureCombiner.library.supportiveGestures, false, true));
            Debug.Log("system: "+ Debugger.dictionaryToString(_player.gestureCombiner.library.systemGestures, false, true));
        };
    }
}
