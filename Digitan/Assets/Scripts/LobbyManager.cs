using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject LobbyName;
    [SerializeField] private GameObject ListPanel;
    [SerializeField] private GameObject PlayerDetails;
    private Lobby lobby;
    private int currentNumberOfPlayers = 0;
    private float updateTimer;


    // Start is called before the first frame update
    void Start()
    {
        //System.Threading.Thread.Sleep(1000);
        var currentLobby = GameObject.FindGameObjectsWithTag("Lobby")[0];

        if (currentLobby.GetComponent<HostLobby>())
        {
            lobby = GameObject.FindGameObjectsWithTag("Lobby")[0].GetComponent<HostLobby>().lobby;
        }
        else
        {
            lobby = GameObject.FindGameObjectsWithTag("Lobby")[0].GetComponent<LobbyDetails>().lobby;

        }

        LobbyName.GetComponent<TextMeshProUGUI>().text = lobby.Name;

        drawPlayers();
        currentNumberOfPlayers = lobby.Players.Count;
    }


    // Update is called once per frame
    void Update()
    {

    }

    private async void PollLobby()
    {
        if (lobby != null)
        {
            updateTimer -= Time.deltaTime;
            if (updateTimer < 0f)
            {
                float updateTimerMax = 3;
                updateTimer = updateTimerMax;

                lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);

                if(lobby.Players.Count != currentNumberOfPlayers)
                {
                    drawPlayers();
                    currentNumberOfPlayers = lobby.Players.Count;
                }
            }
        }
    }

    private void drawPlayers()
    {
        int i = 0;
        foreach (Player player in lobby.Players)
        {
            PlayerDetails.transform.Find("PlayerName").gameObject.GetComponent<TextMeshProUGUI>().text = "xd";

            GameObject playerDetails = Instantiate(
                PlayerDetails,
                new Vector3(0, i * -150 + 225, 0),
                ListPanel.transform.rotation
            );

            playerDetails.transform.SetParent(ListPanel.transform, false);

            i++;
        }
    }
}
