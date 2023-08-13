using System.Collections;
using System.Collections.Generic;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using UnityEngine;
using Zenject;

public class FightSceneController : MonoBehaviour
{
    public bool MoveOnAwake = false;
    
    private Player _player;
    private GestureCombiner _gestureCombiner;
    
    [Inject]
    private void Construct(Player player, GestureCombiner gestureCombiner)
    {
        _player = player;
        _gestureCombiner = gestureCombiner;
        _gestureCombiner.AddRecognitionButton("Start Recognition - Button");
        _gestureCombiner.TestRecognitionFunction();
        if (MoveOnAwake)
        {
            _player.movement.StartMove();
        }
    }

}
