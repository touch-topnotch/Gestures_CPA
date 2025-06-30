using System.Collections.Generic;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using Scripts.Players;
using UnityEngine;


public class TestController : MonoBehaviour
{
    [SerializeField] private RingMenu ringMenu;
    private GesturesLibrary _library;

    private void Start()
    {
        _library = PlayerData.local.gesturesLibrary;
    }


    public void SimulateFrame(string e)
    {
        Debug.Log("Here should be function to play gesture frame" + e);
    }

    public void SimulateGesture(string e)
    {
        Debug.Log("Here should be function to play dynamic gesture " + e);
    }
}