using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class MatchmakerClient : NetworkBehaviour
{
    [SerializeField] private List<GameObject> offObjects = new();
    private NetworkManager _networkManager;
    private string _ticketID;

    private void Start()
    {
        _networkManager = NetworkManager.Singleton;
    }

    private void OnEnable()
    {
        MultiplayerServerStarter.ClientInstance += SignIn;
    }


    private void OnDisable()
    {
        MultiplayerServerStarter.ClientInstance -= SignIn;
    }

    async void SignIn()
    {
        await ClientSignIn("player");
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    async Task ClientSignIn(string serviceProfileName = null)
    {
        if (serviceProfileName != null)
        {
            serviceProfileName = $"{serviceProfileName}";

            var initOptions = new InitializationOptions();
            initOptions.SetProfile(serviceProfileName);
            await UnityServices.InitializeAsync(initOptions);
        }
        else
        {
            await UnityServices.InitializeAsync();
        }

        Debug.Log($"Signed In anonym as {serviceProfileName}{PlayerID()}");
    }

    private string PlayerID()
    {
        return AuthenticationService.Instance.PlayerId;
    }

    public void StartClient()
    {
        CreateATicket();
    }

    private async void CreateATicket()
    {
        var options = new CreateTicketOptions("MagicTest");

        var players = new List<Player>
        {
            new Player(PlayerID())
        };

        var ticketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, options);
        _ticketID = ticketResponse.Id;
        Debug.Log($"Ticket ID: {_ticketID}");
        PollTicketStatus();
    }

    private async void PollTicketStatus()
    {
        MultiplayAssignment multiplayAssignment= null;
        bool gotAssignment = false;
        do
        {
            await Task.Delay(TimeSpan.FromSeconds(1f));
            var ticketStatus = await MatchmakerService.Instance.GetTicketAsync(_ticketID);
            if (ticketStatus == null) continue;
            if (ticketStatus.Type == typeof(MultiplayAssignment))
            {
                multiplayAssignment = ticketStatus.Value as MultiplayAssignment;
            }

            switch (multiplayAssignment.Status)
            {
                case MultiplayAssignment.StatusOptions.Found:
                    gotAssignment = true;
                    TicketAssigned(multiplayAssignment);
                    Debug.Log("MultiplayAssignment.StatusOptions.Found");
                    offObjects.ForEach(e => e.SetActive(false));
                    break;
                case MultiplayAssignment.StatusOptions.InProgress:
                    Debug.Log("MultiplayAssignment.StatusOptions.InProgress");
                    break;
                case MultiplayAssignment.StatusOptions.Failed:
                    gotAssignment = true;
                    Debug.LogError($"Failed to get ticket status. Error :{multiplayAssignment.Message}");
                    break;
                case MultiplayAssignment.StatusOptions.Timeout:
                    gotAssignment = true;
                    Debug.LogError($"Failed to get ticket status. Ticket timed out.");
                    break;
                default:
                    throw new InvalidOperationException();
            }
        } while (!gotAssignment);
    }

    private void OnApplicationQuit()
    {
        if (Application.platform != RuntimePlatform.LinuxServer)
        {
            if (_networkManager.IsConnectedClient)
            {
                _networkManager.Shutdown(true);
                _networkManager.DisconnectClient(_networkManager.LocalClientId);
            }
        }
    }

    private void TicketAssigned(MultiplayAssignment assignment)
    {
        Debug.Log($"Ticket Assigned: {assignment.Ip}:{assignment.Port}");
        _networkManager.GetComponent<UnityTransport>().SetConnectionData(assignment.Ip, (ushort)assignment.Port);
        _networkManager.StartClient();
    }
}