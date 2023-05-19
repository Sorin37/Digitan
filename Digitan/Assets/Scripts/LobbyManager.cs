using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject LobbyName;
    [SerializeField] private GameObject ListPanel;
    [SerializeField] private GameObject PlayerDetails;
    [SerializeField] private Button PlayButton;
    public Lobby lobby;
    private int currentNumberOfPlayers = 0;
    private float updateTimer = 5;
    private float checkStartTimer = 5;
    private bool joined = false;

    private void Awake()
    {
        PlayButton.onClick.AddListener(async () =>
        {
            string joinCode = await CreateRelay();
            await UpdateLobbyRelayCode(joinCode);
            SceneManager.LoadScene("Game");
        });
        tag = "Lobby";
        DontDestroyOnLoad(transform.parent.gameObject);
    }

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
            var toDestroy = GameObject.FindGameObjectsWithTag("Lobby")[0];
            lobby = toDestroy.GetComponent<LobbyDetails>().lobby;
            Destroy(toDestroy);
        }

        LobbyName.GetComponent<TextMeshProUGUI>().text = lobby.Name;

        DrawPlayers();
        currentNumberOfPlayers = lobby.Players.Count;

        if (currentNumberOfPlayers > 1)
        {
            PlayButton.gameObject.SetActive(false);
        }
    }


    // Update is called once per frame
    void Update()
    {
        PollLobby();
        CheckStartLobby();
    }

    private async void PollLobby()
    {
        try
        {
            if (lobby != null)
            {
                updateTimer -= Time.deltaTime;
                if (updateTimer < 0f)
                {
                    float updateTimerMax = 5;
                    updateTimer = updateTimerMax;

                    lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);

                    if (lobby.Players.Count != currentNumberOfPlayers)
                    {
                        DrawPlayers();
                        currentNumberOfPlayers = lobby.Players.Count;
                    }
                }
            }
        }
        catch (LobbyServiceException)
        {

            Debug.LogWarning("LSE in PollLobby (GetLobbyAsync) in LobbyManager");
        }
    }

    private void DrawPlayers()
    {
        foreach (Transform child in ListPanel.transform)
        {
            Destroy(child.gameObject);
        }

        int i = 0;
        foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            PlayerDetails.transform.Find("PlayerName")
                .gameObject.GetComponent<TextMeshProUGUI>()
                .text = player.Data["PlayerName"].Value;

            GameObject playerDetails = Instantiate(
                PlayerDetails,
                new Vector3(0, i * -150 + 225, 0),
                ListPanel.transform.rotation
            );

            playerDetails.transform.SetParent(ListPanel.transform, false);

            i++;
        }
    }

    private async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.LogError(joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().
                SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartHost();

            return joinCode;
        }
        catch (RelayServiceException ex)
        {
            Debug.LogError(ex.Message);
            throw;
        }
    }

    private async Task JoinRelay(string relayCode)
    {
        if (joined)
            return;
        try
        {   
            joined = true;
            Debug.Log("Joining Relay with " + relayCode);
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayCode);
            var relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();

        }
        catch (RelayServiceException ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    private async void CheckStartLobby()
    {

        try
        {
            if (lobby == null)
                return;

            checkStartTimer -= Time.deltaTime;

            if (checkStartTimer < 0f)
            {
                float checkStartTimerMax = 5;
                checkStartTimer = checkStartTimerMax;

                if (lobby.Data["RelayCode"].Value != "None")
                {
                    await JoinRelay(lobby.Data["RelayCode"].Value);
                    SceneManager.LoadScene("Game");
                }
            }
        }
        catch (LobbyServiceException)
        {
            Debug.LogWarning("Lobby Service Error in CheckStartLobby (JoinStartLobby) in Lobby Manager");
        }
    }

    private async Task UpdateLobbyRelayCode(string relayCode)
    {
        try
        {
            lobby = await Lobbies.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {"RelayCode", new DataObject(DataObject.VisibilityOptions.Public, relayCode) },
                }
            });
        }
        catch (LobbyServiceException)
        {
            Debug.LogWarning("Lobby Service Error in UpdateLobbyRelayCode (UpdateLobbyAsync) in LobbyManager");
        }
    }

    private void UpdateLobbies(Lobby lobby)
    {
        var currentLobby = GameObject.FindGameObjectsWithTag("Lobby")[0];

        if (currentLobby.GetComponent<HostLobby>())
        {
            GameObject.FindGameObjectsWithTag("Lobby")[0].GetComponent<HostLobby>().lobby = lobby;
        }
        else
        {
            GameObject.FindGameObjectsWithTag("Lobby")[0].GetComponent<LobbyDetails>().lobby = lobby;
        }
    }

    private Lobby GetLobby()
    {
        var currentLobby = GameObject.FindGameObjectsWithTag("Lobby")[0];

        if (currentLobby.GetComponent<HostLobby>())
        {
            return GameObject.FindGameObjectsWithTag("Lobby")[0].GetComponent<HostLobby>().lobby;
        }
        else
        {
            return GameObject.FindGameObjectsWithTag("Lobby")[0].GetComponent<LobbyDetails>().lobby;
        }
    }
}
