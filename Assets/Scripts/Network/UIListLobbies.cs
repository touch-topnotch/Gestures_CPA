using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Network
{
    public class UIListLobbies : MonoBehaviour
    {
        public event Action<string> LobbyChosenForConnect;
        
        [SerializeField] private UILobby uILobbyPrefab;
        [SerializeField] private Transform parentTransform;

        private Dictionary<string, UILobby> _uiLobbies = new();

        public void OnEnable()
        {
            foreach (var uiLobby in _uiLobbies)
            {
                uiLobby.Value.Clicked += OnLobbyClicked;
            }
        }

        public void OnDisable()
        {
            foreach (var uiLobby in _uiLobbies)
            {
                uiLobby.Value.Clicked -= OnLobbyClicked;
            }
        }

        public void UpdateLobbies(List<LobbyInfo> uiLobbiesInfo)
        {
            Dictionary<string, UILobby> activeUILobbies = new();
            foreach (var uiLobbyInfo in uiLobbiesInfo)
            {
                if (!_uiLobbies.ContainsKey(uiLobbyInfo.LobbyID))
                {
                    var uiLobby = Instantiate<UILobby>(uILobbyPrefab, parentTransform);
                    uiLobby.Clicked += OnLobbyClicked;
                    _uiLobbies.Add(uiLobbyInfo.LobbyID, uiLobby);
                }

                activeUILobbies.Add(uiLobbyInfo.LobbyID, _uiLobbies[uiLobbyInfo.LobbyID]);
                _uiLobbies[uiLobbyInfo.LobbyID].UpdateLobby(uiLobbyInfo);
            }

            _uiLobbies.Except(activeUILobbies).ToList()
                .ForEach(a => a.Value.gameObject.SetActive(false));
        }

        public void OnLobbyClicked(string lobbyID)
        {
            LobbyChosenForConnect?.Invoke(lobbyID);
        }
    }
}