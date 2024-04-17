using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILobby : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private TextMeshProUGUI lobbyPlayersCount;
    [SerializeField] private Image background;
    [SerializeField] private GameObject connectButton;
    
    public Color MyLobby;
    public Color NotMyLobby;
    
    private string _lobbyID;
    
    public event Action<string> Clicked;
    
    public void UpdateLobby(LobbyInfo lobbyInfo)
    {
        _lobbyID = lobbyInfo.LobbyID;
        lobbyName.text = lobbyInfo.LobbyID;
        lobbyPlayersCount.text = $"{lobbyInfo.CurrentPlayersCount}/{lobbyInfo.MaxPlayersCount}";
        background.color = lobbyInfo.IsMineLobby ? MyLobby : NotMyLobby;
        connectButton.SetActive(!lobbyInfo.IsMineLobby);
    }

    public void OnClicked()
    {
        Clicked?.Invoke(_lobbyID);
    }
}