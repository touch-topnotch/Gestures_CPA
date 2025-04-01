using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Network
{
    public class LobbyConnector : MonoBehaviour
    {
        private Lobby _hostLobby;
        private Lobby _currentLobby;
        private float _heartbeatTimer = 0;
        private const float HEART_BEAT_TIMER_MAX = 15f;
        private const int MAX_PLAYERS = 4;
        public event Action LobbyConnectingStarted;
        public event Action<string> LobbyConnectingCompleted;
        public bool IsLobbyHost => _hostLobby == _currentLobby;
       

        public async Task Initialize()
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private void Update()
        {
            HandleLobbyHeartbeat();
        }

        public async Task<string> CheckData()
        {
            _currentLobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
            if (_currentLobby.Data == null)
                return "";

            if (!_currentLobby.Data.ContainsKey("relayCode"))
                return "";

            var joinCode = _currentLobby.Data["relayCode"];
            return joinCode.Value;
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
            await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, AuthenticationService.Instance.PlayerId);
        }

        public async void SendRelayCode(string code)
        {
            var data = new Dictionary<string, DataObject>
            {
                ["relayCode"] = new(DataObject.VisibilityOptions.Member, code)
            };
            await LobbyService.Instance.UpdateLobbyAsync(_hostLobby.Id, new UpdateLobbyOptions()
            {
                Data = data
            });
        }

        public async Task ConnectOrCreateLobby()
        {
            if (_currentLobby != null)
                return;

            LobbyConnectingStarted?.Invoke();

            var availableLobbies = await ListLobbies();

            if (availableLobbies.Count > 0)
            {
                Debug.Log(availableLobbies[0].Name);
                await QuickJoinLobby();
            }
            else
            {
                await CreateLobby();
            }

            if (_currentLobby != null) LobbyConnectingCompleted?.Invoke(_currentLobby.Id);
        }

        public void OnLobbyChanged()
        {
            
        }

        private async Task CreateLobby()
        {
            try
            {
                var lobbyName = "MyLobby" + AuthenticationService.Instance.PlayerId;
                var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYERS).ConfigureAwait(false);
                Debug.Log("created Lobby" + lobby.Name + lobby.MaxPlayers);
                _hostLobby = lobby;
                _currentLobby = lobby;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
            }
        }

        private async Task<List<Lobby>> ListLobbies()
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

        private async Task QuickJoinLobby()
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
    }
}