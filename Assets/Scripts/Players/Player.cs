using System;
using System.Collections;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.Hands;
using Scripts.Movements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using Zenject;

namespace Scripts.PlayerLogic
{
    public enum GameState
    {
        Menu,
        Fight,
    }
    public enum RuntimeXRInteractor
    {
        OpenXR,
        Debugger,
    }

    public class OnGameStateChanged: UnityEvent<GameState>{}
    
    [RequireComponent(typeof(SupportHandCreator))]
    
    public abstract class Player : MonoBehaviour
    {
        public RuntimeXRInteractor xrInteractor;
        
        public OnGameStateChanged GameStateChanged;
        
        public UserHands playerHands;
        public SupportHandCreator supHandCreator { get; private set; }
        
        public Movement movement;
 
        private GesturesLibrary _library;
        private GameState _currentGameState;
        
        public GameState currentGameState
        {
            get => _currentGameState;
            set
            {
                _currentGameState = value;
                GameStateChanged?.Invoke(currentGameState);
            }
        }
        
        [Inject]
        private void Construct(GesturesLibrary library)
        {
            _library = library;
        }
        public virtual void Initialize(){
           
            
            supHandCreator = GetComponent<SupportHandCreator>();
            currentGameState = GameState.Menu;
            
            playerHands.Initialize();
            _library.InitializeAllAssets(playerHands);
            
            Debug.Log($"{xrInteractor} has initialized");
        }
    }

}
