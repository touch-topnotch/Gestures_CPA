using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Network;
using Newtonsoft.Json;
using Scripts.PlayerLogic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using UnityEngine;
using Player = Unity.Services.Matchmaker.Models.Player;


public class MultiplayerServerStarter : NetworkBehaviour
{
    public static event Action ClientInstance;

    private const string INTERNAL_SERVER_IP = "0.0.0.0";
    private string _externalServerIP = "0.0.0.0";

    private string _externalConnectionString => $"{_externalServerIP}:{_serverPort}";

    private ushort _serverPort = 7777;
    private IMultiplayService _multiplayService;
    private const int MULTIPLAY_SERVICE_TIMEOUT = 20000;

    private string _allocationID;
    private MultiplayEventCallbacks _serverCallbacks;

    private IServerEvents _serverEvents;
    private BackfillTicket _localBlackfillTicket;

    private CreateBackfillTicketOptions _createBackfillTicketOptions;
    private const int _ticketCheckMs = 1000;
    private MatchmakingResults _matchmakingPayload;

    private bool _backfilling = false;
    private NetworkManager _networkManager;

    public bool playerWasConnected = false;

    async void Start()
    {
        _networkManager = NetworkManager.Singleton;
        bool server = false;
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "-dedicatedServer")
                server = true;

            if (args[i] == "-port" && (i + 1 < args.Length))
                _serverPort = (ushort)int.Parse(args[i + 1]);

            if (args[i] == "-ip" && (i + 1 < args.Length))
            {
                _externalServerIP = args[i + 1];
            }
        }

        if (server)
        {
            StartServer();
            await StartServerServices();
        }
        else
        {
            ClientInstance?.Invoke();
        }
    }

    private void StartServer()
    {
        _networkManager.GetComponent<UnityTransport>().SetConnectionData(
            INTERNAL_SERVER_IP, _serverPort);
        _networkManager.StartServer();
        _networkManager.OnClientDisconnectCallback += ClientDisconnected;
        _networkManager.OnClientConnectedCallback += ClientConnected;
    }

    private void ClientConnected(ulong obj)
    {
        playerWasConnected = true;
    }

    private void ClientDisconnected(ulong obj)
    {
        Debug.Log($"{obj} player disconnected");
        if (!_backfilling && _networkManager.ConnectedClients.Count > 0 && NeedsPlayers())
        {
            Debug.Log($"{obj} Started BeginBackfilling");
            BeginBackfilling(_matchmakingPayload);
        }
    }

    private void Update()
    {
        if (!playerWasConnected)
            return;

        if (Application.platform == RuntimePlatform.LinuxServer)
        {
            if (_networkManager.ConnectedClients.Count == 0)
            {
                enabled = false;
                Debug.Log("APPLICATION QUIT");
                Application.Quit();
            }
        }
    }

    private async UniTask StartServerServices()
    {
        await UnityServices.InitializeAsync();
        try
        {
            _multiplayService = MultiplayService.Instance;
            await _multiplayService.StartServerQueryHandlerAsync(4, "n/a", "n/a", "0", "n/a");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Something went wrong:\n{e}");
        }

        try
        {
            var matchmakerPayload = await GetMatchmakerPayload(MULTIPLAY_SERVICE_TIMEOUT);
            if (matchmakerPayload != null)
            {
                Debug.Log($"Got payload: {matchmakerPayload}");
                await StartBackfill(matchmakerPayload);
            }
            else
            {
                Debug.LogWarning($"Getting the Matchmaker Payload timed out, starting with defaults.");
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Something went wrong:\n{e}");
        }
    }

    private async Task StartBackfill(MatchmakingResults payload)
    {
        var backfillProperties = new BackfillTicketProperties(payload.MatchProperties);
        _localBlackfillTicket = new BackfillTicket
            { Id = payload.MatchProperties.BackfillTicketId, Properties = backfillProperties };
        await BeginBackfilling(payload);
    }

    private void Dispose()
    {
        _serverCallbacks.Allocate -= OnMultiplayAllocation;
        _serverEvents?.UnsubscribeAsync();
    }

    private async Task BeginBackfilling(MatchmakingResults payload)
    {
        var matchProperties = payload.MatchProperties;
        _createBackfillTicketOptions = new CreateBackfillTicketOptions()
        {
            Connection = _externalConnectionString,
            QueueName = payload.QueueName,
            Properties = new BackfillTicketProperties(matchProperties)
        };
        if (string.IsNullOrEmpty(_localBlackfillTicket.Id))
            _localBlackfillTicket.Id =
                await MatchmakerService.Instance.CreateBackfillTicketAsync(_createBackfillTicketOptions);

        _backfilling = true;

#pragma warning  disable 4014
        BackfillLoop();
#pragma warning  restore 4014
    }

    private async Task BackfillLoop()
    {
        while (_backfilling && NeedsPlayers())
        {
            _localBlackfillTicket =
                await MatchmakerService.Instance.ApproveBackfillTicketAsync(_localBlackfillTicket.Id);
            if (!NeedsPlayers())
            {
                await MatchmakerService.Instance.DeleteBackfillTicketAsync(_localBlackfillTicket.Id);
                _localBlackfillTicket.Id = null;
                _backfilling = false;
                return;
            }

            await Task.Delay(_ticketCheckMs);
        }

        _backfilling = false;
    }

    private bool NeedsPlayers()
    {
        return _networkManager.ConnectedClients.Count < 4;
    }

    private async UniTask<MatchmakingResults> GetMatchmakerPayload(int timeout)
    {
        var matchmakerPayloadTask = SubscribeAndAwaitMatchmakerAllocation();
        if (await Task.WhenAny(matchmakerPayloadTask, Task.Delay(timeout)) == matchmakerPayloadTask)
        {
            return matchmakerPayloadTask.Result;
        }

        return null;
    }

    private async Task<MatchmakingResults> SubscribeAndAwaitMatchmakerAllocation()
    {
        if (_multiplayService == null) return null;

        _allocationID = null;
        _serverCallbacks = new MultiplayEventCallbacks();
        _serverCallbacks.Allocate += OnMultiplayAllocation;
        _serverEvents = await _multiplayService.SubscribeToServerEventsAsync(_serverCallbacks);

        _allocationID = await AwaitAllocationID();

        var mmPayload = await GetMatchmakerAllocationPayloadAsync();
        return mmPayload;
    }

    private async Task<MatchmakingResults> GetMatchmakerAllocationPayloadAsync()
    {
        try
        {
            var payloadAllocation =
                await MultiplayService.Instance.GetPayloadAllocationFromJsonAs<MatchmakingResults>();
            var modelAsJson = JsonConvert.SerializeObject(payloadAllocation, Formatting.Indented);
            Debug.Log($"{nameof(GetMatchmakerAllocationPayloadAsync)}:\n{modelAsJson}");
            return payloadAllocation;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Something went wrong:\n{e}");
        }

        return null;
    }

    private async Task<string> AwaitAllocationID()
    {
        var config = _multiplayService.ServerConfig;
        Debug.Log($"Awaiting Allocation. Server Config is: " +
                  $"-ServerID: {config.ServerId}\n" +
                  $"-AllocationID: {config.AllocationId}\n" +
                  $"-Port: {config.Port}\n" +
                  $"-QPort: {config.QueryPort}\n" +
                  $"-logs: {config.ServerLogDirectory}");
        while (string.IsNullOrEmpty(_allocationID))
        {
            var configID = config.AllocationId;
            if (!string.IsNullOrEmpty(configID) && string.IsNullOrEmpty(_allocationID))
            {
                _allocationID = configID;
                break;
            }

            await Task.Delay(100);
        }

        return _allocationID;
    }

    private void OnMultiplayAllocation(MultiplayAllocation allocation)
    {
        Debug.Log($"OnAllocation: {allocation.AllocationId}");
        if (string.IsNullOrEmpty(allocation.AllocationId)) return;
        _allocationID = allocation.AllocationId;
    }
}