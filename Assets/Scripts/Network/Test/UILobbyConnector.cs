using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;
using Scripts.Events;
using Scripts.Static;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;

namespace Network.Test
{
    public class UILobbyConnector : MonoBehaviour
    {
        [SerializeField] private bool CreateOnAwake;
        [SerializeField] private GameObject LobbyComponents;
        [SerializeField] private LobbyConnector lobbyConnector;
        [SerializeField] private UIListLobbies uiListLobbies;
        [SerializeField] private GameObject LobbyActionVariants;
        [SerializeField] private Loading loading;

        private const int MAX_PLAYERS = 4;
        private const float UPDATE_PERIOD = 4;

        private float _currentTime = 3f;
        private bool _isUpdating = false;

        private void Awake()
        {
            if (CreateOnAwake || Application.platform == RuntimePlatform.Android)
            {
                Global.eventManager.onServicesInitilalised += CreateLobby;
            }
        }

        private void OnEnable()
        {
            uiListLobbies.LobbyChosenForConnect += OnLobbyChosenForConnect;
        }

        private async void OnLobbyChosenForConnect(string obj)
        {
            LobbyComponents.SetActive(false);
            await lobbyConnector.ConnectLobby(obj);
        }

        private void OnDisable()
        {
            uiListLobbies.LobbyChosenForConnect += OnLobbyChosenForConnect;
        }

        public async void CreateLobby()
        {
            if (!AuthenticationService.Instance.IsAuthorized)
                return;

            LobbyActionVariants.SetActive(false);
            loading.gameObject.SetActive(true);
            await lobbyConnector.CreateLobby(MAX_PLAYERS);

            await UniTask.SwitchToMainThread();

            NetworkManager.Singleton.OnClientStarted += () => { LobbyComponents.SetActive(false); };

            ShowLobbies();
        }

        private async void Update()
        {
            if (!_isUpdating)
                return;

            _currentTime += Time.deltaTime;
            if (_currentTime > UPDATE_PERIOD)
            {
                _currentTime = 0;
                await UpdateListOfLobbies();
                _isUpdating = false;
            }
        }

        public void ShowLobbies()
        {
            if (!AuthenticationService.Instance.IsAuthorized)
                return;
            loading.gameObject.SetActive(false);
            LobbyActionVariants.SetActive(false);
            _isUpdating = true;
        }

        private async UniTask UpdateListOfLobbies()
        {
            var lobbies = await lobbyConnector.ListLobbies();

            var lobbiesInfo = lobbies.Select(
                a =>
                    new LobbyInfo(
                        a.Id,
                        a.Players.Count,
                        a.MaxPlayers,
                        lobbyConnector.CurrentLobbyID == a.Id)
            ).ToList();

            uiListLobbies.UpdateLobbies(lobbiesInfo);
        }
    }
}