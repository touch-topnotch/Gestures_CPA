using System.Collections;
using System.Collections.Generic;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using UnityEngine;
using Zenject;

public class FightSceneController : MonoBehaviour
{
    private Player _player;
    private GestureCombiner _gestureCombiner;
    
    [Inject]
    private void Construct(Player player, GestureCombiner gestureCombiner)
    {
        _player = player;
        _gestureCombiner = gestureCombiner;
    }

    private void Start()
    {
        _player.movement.StartMove();
        _player.currentGameState = GameState.Fight;

        _gestureCombiner.AddRecognitionButton("Start Recognition - Button");
    }

}
