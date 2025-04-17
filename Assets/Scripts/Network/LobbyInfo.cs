using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyInfo
{
    public string LobbyID { get; private set; }
    public int CurrentPlayersCount { get; private set; }
    public int MaxPlayersCount { get; private set; }
    public bool IsMineLobby { get; private set; }

    public LobbyInfo(string lobbyID, int currentPlayersCount, int maxPlayersCount, bool isMineLobby)
    {
        LobbyID = lobbyID;
        CurrentPlayersCount = currentPlayersCount;
        MaxPlayersCount = maxPlayersCount;
        IsMineLobby = isMineLobby;
    }
}