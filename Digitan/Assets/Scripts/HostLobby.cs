using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEditor;

public class HostLobby : MonoBehaviour
{
    [SerializeField] private Button hostButton;

    public Lobby lobby;
    private float heartbeatTimer;
    private float pollTimer;

    private void Awake()
    {
        tag = "Lobby";
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    public async void InitializeUnityServices()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.LogError("Sign in " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        hostButton.interactable = true;
    }

    public async Task CreateLobby(string lobbyName, string nickname)
    {
        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Data = new Dictionary<string, DataObject> {
                    { "RelayCode", new DataObject(DataObject.VisibilityOptions.Public, "None") }
            },
            Player = new Unity.Services.Lobbies.Models.Player
            {
                Data = new Dictionary<string, PlayerDataObject> {
                    { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, nickname) },
                }
            }
        };

        lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 4, options);
    }

    // Update is called once per frame
    void Update()
    {
        KeepLobbyAlive();
    }

    private async void KeepLobbyAlive()
    {
        if (lobby == null)
            return;

        heartbeatTimer -= Time.deltaTime;
        if (heartbeatTimer < 0)
        {
            float heartbeatTimerMax = 15;
            heartbeatTimer = heartbeatTimerMax;

            await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
        }
    }

}
