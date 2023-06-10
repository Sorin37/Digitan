using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyDetails : MonoBehaviour
{
    public Lobby lobby;
    private float pollTimer;
    [SerializeField] GameObject lobbyExceptionCanvas;

    // Start is called before the first frame update
    void Start()
    {
        tag = "Lobby";
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        PollLobby();
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

                try
                {
                    lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
                }
                catch (LobbyServiceException)
                {
                    lobbyExceptionCanvas.transform.Find("LobbyExceptionManager").GetComponent<LobbyExceptionManager>().SetErrorMessage(
                    "Poor internet connection, please try again!"
                    );
                    lobbyExceptionCanvas.SetActive(true);
                }
            }
        }
    }
}
