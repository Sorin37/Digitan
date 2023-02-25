using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

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

        hostButton.interactable= true;
    }

    public async Task CreateLobby(string lobbyName)
    {
        lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 4);
    }

    // Update is called once per frame
    void Update()
    {
        KeepLobbyAlive();
        PollLobby();
    }

    private async void KeepLobbyAlive()
    {
        if(lobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer < 0)
            {
                float heartbeatTimerMax = 15;
                heartbeatTimer = heartbeatTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
            }
        }
    }

    private async void PollLobby()
    {
        if (lobby != null)
        {
            pollTimer -= Time.deltaTime;
            if (pollTimer < 0f)
            {
                float pollTimerMax = 1.1f;
                pollTimer = pollTimerMax;

                await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
            }
        }
    }
}
