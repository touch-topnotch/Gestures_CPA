using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Events;

namespace Network
{
    public class LobbyConnector : MonoBehaviour
    {
        private Lobby _currentLobby;
        private float _heartbeatTimer = 0;
        private const float HEART_BEAT_TIMER_MAX = 15f;
        private const string RELAY_CODE_KEY = "RelayCode";
        public string CurrentLobbyID { get; private set; }

        public event Action<string> LobbyConnected;
        public event Action<string> LobbyConnectedAsHost;
        public UnityEvent onWaitingToConnect;
        private void Update()
        {
            HandleLobbyHeartbeat();
        }

        private async void HandleLobbyHeartbeat()
        {
            if (_currentLobby != null)
            {
                _heartbeatTimer -= Time.deltaTime;
                if (_heartbeatTimer < 0f)
                {
                    _heartbeatTimer = HEART_BEAT_TIMER_MAX;

                    await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
                }
            }
        }

        private async void OnApplicationQuit()
        {
            if (_currentLobby != null)
                await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id,
                    AuthenticationService.Instance.PlayerId);
        }

        public async UniTask ConnectLobby(string id)
        {
            onWaitingToConnect?.Invoke();
            if (_currentLobby != null)
            {
                string playerId = AuthenticationService.Instance.PlayerId;
                await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, playerId);
            }

            _currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(id);
            Debug.Log($"connected Lobby {_currentLobby.Name} {_currentLobby.Players.Count}/{_currentLobby.MaxPlayers}");
            CurrentLobbyID = _currentLobby.Id;
            
            LobbyConnected?.Invoke(CurrentLobbyID);
        }

        public async UniTask ConnectOrCreateLobby(int maxPlayers)
        {
            
            if (_currentLobby != null)
                return;
     

            var availableLobbies = await ListLobbies();

            if (availableLobbies.Count > 0)
            {
                await QuickJoinLobby();
            }
            else
            {
                await CreateLobby(maxPlayers);
            }
        }


        public async UniTask CreateLobby(int maxPlayers)
        {
            try
            {
                onWaitingToConnect?.Invoke();
                var lobbyName = $"MyLobby{AuthenticationService.Instance.PlayerId}";
                var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers).ConfigureAwait(false);
                Debug.Log($"created Lobby {lobby.Name} {lobby.MaxPlayers}");
                _currentLobby = lobby;
                CurrentLobbyID = _currentLobby.Id;
              
                LobbyConnectedAsHost?.Invoke(CurrentLobbyID);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }

        public async UniTask<List<Lobby>> ListLobbies()
        {
            try
            {
                var queryLobbiesOptions = new QueryLobbiesOptions()
                {
                    Count = 25,

                    Filters = new List<QueryFilter>()
                    {
                        new(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),
                    },
                    Order = new List<QueryOrder>()
                    {
                        new(false, QueryOrder.FieldOptions.Created),
                    }
                };

                var queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);
                return queryResponse.Results;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
                return new List<Lobby>();
            }
        }

        private async UniTask QuickJoinLobby()
        {
            try
            {
                var lobby = await Lobbies.Instance.QuickJoinLobbyAsync();

                _currentLobby = lobby;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }

        public void SetCodeAsRelayCode(string code)
        {
            var newData = new Dictionary<string, DataObject>();
            newData.Add(RELAY_CODE_KEY, new DataObject(DataObject.VisibilityOptions.Member, code));
            LobbyService.Instance.UpdateLobbyAsync(CurrentLobbyID, new UpdateLobbyOptions()
            {
                Data = newData
            });
        }
    }
}