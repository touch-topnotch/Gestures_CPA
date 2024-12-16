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
        _gestureCombiner.TestRecognitionFunction();
    }

}
