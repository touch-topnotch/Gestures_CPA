using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace Network.Test
{
    public class ConnectionTestVariant : MonoBehaviour
    {
        [SerializeField] private LobbyConnector lobbyConnector;
        [SerializeField] private RelayConnector relayConnector;
        private const int MAX_PLAYERS = 4;
        private async void Start()
        {
            await lobbyConnector.Initialize();
            await lobbyConnector.ConnectOrCreateLobby();
            if (lobbyConnector.IsLobbyHost)
            {
                var joinCode = await relayConnector.CreateRelay(MAX_PLAYERS);
                lobbyConnector.SendRelayCode(joinCode);
                await Test();
            }
            else
            {
                await AwaitingRelayCode();
            }
        }



        private async Task Test()
        {
            while (true)
            {
                var e = await lobbyConnector.CheckData();
                if (string.IsNullOrEmpty(e))
                {
                    await Task.Delay(1000);
                }
                else
                {
                    Debug.Log(e);
                    return;
                }
            }
        }

        private async Task AwaitingRelayCode()
        {
            while (true)
            {
                var e = await lobbyConnector.CheckData();
                
                if (string.IsNullOrEmpty(e))
                {
                    await Task.Delay(1000);
                }
                else
                {
                    relayConnector.JoinRelay(e);
                    return;
                }
            }
        }
    }
}