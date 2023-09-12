using Scripts.Events;
using Scripts.Gestures;
using Scripts.Hands;
using Scripts.Movements;
using UnityEngine;
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

        public Movement movement;

        public NetworkUser ownUser;

        private GesturesLibrary _library;
        private GameState _currentGameState;
        private UpdateEvent _onUpdate;

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
        private void Construct(GesturesLibrary library, UpdateEvent onUpdate)
        {
            _library = library;
            _onUpdate = onUpdate;
        }

        public virtual void Initialize()
        {
            currentGameState = GameState.Menu;
            playerHands.Initialize();
            _library.InitializeAllAssets(playerHands);
            Debug.Log($"{xrInteractor} has initialized");
        }

        public virtual void ConnectToUser(NetworkUser networkUser)
        {
            ownUser = networkUser;
            transform.position = ownUser.transform.position;
            ownUser.parenter.SetParent(transform, ref _onUpdate);
            print($"The {transform.name} connected to {ownUser.transform.name} with id {ownUser.networkObject.OwnerClientId}");
        }
    }
}
