using System;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.Hands;
using Scripts.Movements;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Scripts.PlayerLogic
{
 

    public class OnGameStateChanged: UnityEvent<GameState>{}
    public abstract class Player : MonoBehaviour
    {
        [Header("Game settings")]
        public RuntimeXRInteractor xrInteractor;
        public OnGameStateChanged gameStateChanged;

        [Header("Body parts")] public BodyAnchors bodyAnchors;
        
        [Header("Scripts")]
        public Movement movement;
        [HideInInspector] public NetworkUser ownUser;

        private GesturesLibrary _library;
        private GameState _currentGameState;
        private UpdateEvent _onUpdate;

        public GameState currentGameState
        {
            get => _currentGameState;
            set
            {
                _currentGameState = value;
                gameStateChanged?.Invoke(currentGameState);
            }
        }
        
        [Inject]
        private void Construct(GesturesLibrary library, UpdateEvent onUpdate)
        {
            _library = library;
            _onUpdate = onUpdate;
        }

        public virtual void Initialize()
        {
            currentGameState = GameState.Menu;
            bodyAnchors.Hands.Initialize();
            _library.InitializeAllAssets(bodyAnchors.Hands);
            Debug.Log($"{xrInteractor} has initialized");
        }

        public virtual void ConnectToUser(NetworkUser networkUser)
        {
            ownUser = networkUser;
            transform.position = ownUser.transform.position;
            ownUser.bodyParts.Body.SetParent(bodyAnchors.Body, ref _onUpdate);
            ownUser.bodyParts.Head.SetParent(bodyAnchors.Head, ref _onUpdate);
            movement.StartMove();
            print(
                $"The {transform.name} connected to {ownUser.transform.name} with id {ownUser.networkObject.OwnerClientId}");
        }
    }
  
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
}
