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
using static StealManager;
using System.Linq;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject LobbyName;
    [SerializeField] private GameObject ListPanel;
    [SerializeField] private GameObject PlayerDetails;
    [SerializeField] private GameObject ClientDetailsPrefab;
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button MenuButton;
    [SerializeField] private GameObject lobbyExceptionCanvas;
    [SerializeField] private GameObject loadingScreenCanvas;
    [SerializeField] private GameObject kickCanvas;
    public Lobby lobby;
    private int currentNumberOfPlayers = 0;
    private float updateTimer = 5;
    private float checkStartTimer = 5;
    private bool joined = false;
    private bool isHost = false;
    private bool hostLeft = false;

    private void Awake()
    {
        InitPlayButton();
        InitMenuButton();
        tag = "Lobby";
        DontDestroyOnLoad(transform.parent.gameObject);
    }

    private void InitPlayButton()
    {
        PlayButton.onClick.AddListener(async () =>
        {
            if (currentNumberOfPlayers == 1)
            {
                lobbyExceptionCanvas.transform.Find("LobbyExceptionManager").
                    GetComponent<LobbyExceptionManager>().SetErrorMessage(
                        "You can not play by yourself!"
                    );
                lobbyExceptionCanvas.SetActive(true);
                return;
            }

            loadingScreenCanvas.GetComponent<LoadingScreenManager>().SetProgress(1, currentNumberOfPlayers);
            loadingScreenCanvas.SetActive(true);
            string joinCode = await CreateRelay();
            await UpdateLobbyRelayCode(joinCode);
            SceneManager.LoadScene("Game");
        });
    }

    private void InitMenuButton()
    {
        MenuButton.onClick.AddListener(async () =>
        {
            if (!await LeaveLobby())
            {
                return;
            }

            var go = new GameObject("Sacrificial Lamb");
            DontDestroyOnLoad(go);

            foreach (var root in go.scene.GetRootGameObjects())
                Destroy(root);

            SceneManager.LoadScene("MainMenu");
        });
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
        else
        {
            isHost = true;
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
        if (hostLeft)
            return;

        if (lobby != null)
        {
            updateTimer -= Time.deltaTime;
            if (updateTimer < 0f)
            {
                float updateTimerMax = 5;
                updateTimer = updateTimerMax;

                try
                {
                    lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
                }
                catch (LobbyServiceException ex)
                {
                    if (ex.Message == "lobby not found")
                    {
                        hostLeft = true;

                        var lobbyExceptionManager = lobbyExceptionCanvas.transform.Find("LobbyExceptionManager");
                        lobbyExceptionManager.GetComponent<LobbyExceptionManager>().SetErrorMessage(
                            "The host left the lobby!"
                        );

                        //var okButton = lobbyExceptionManager.transform.Find("Panel").transform.Find("OkButton").GetComponent<Button>();

                        //okButton.onClick.RemoveAllListeners();
                        //okButton.onClick.AddListener(() =>
                        //{

                        //});
                    }
                    else
                    {
                        lobbyExceptionCanvas.transform.Find("LobbyExceptionManager")
                            .GetComponent<LobbyExceptionManager>().SetErrorMessage(
                            "Poor internet connection, please try again!"
                        );
                    }

                    lobbyExceptionCanvas.SetActive(true);
                }
                catch (Exception)
                {
                    lobbyExceptionCanvas.transform.Find("LobbyExceptionManager").GetComponent<LobbyExceptionManager>().SetErrorMessage(
                       "No internet connection!"
                   );
                    lobbyExceptionCanvas.SetActive(true);
                }

                if (lobby.Players.Count != currentNumberOfPlayers)
                {
                    GetKickedOut();
                    DrawPlayers();
                    currentNumberOfPlayers = lobby.Players.Count;
                }
            }
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
            var playerName = player.Data["PlayerName"].Value;

            GameObject playerDetails = null;

            if (isHost && i != 0)
            {
                playerDetails = Instantiate(
                    ClientDetailsPrefab,
                    new Vector3(0, i * -150 + 225, 0),
                    ListPanel.transform.rotation
                );

                var kickButton = playerDetails.transform.Find("KickButton")
                    .gameObject.GetComponent<Button>();

                kickButton.onClick.AddListener(async () =>
                {
                    kickButton.interactable = false;
                    await LobbyService.Instance.RemovePlayerAsync(lobby.Id, player.Id);
                });
            }
            else
            {
                playerDetails = Instantiate(
                    PlayerDetails,
                    new Vector3(0, i * -150 + 225, 0),
                    ListPanel.transform.rotation
                );
            }

            playerDetails.transform.SetParent(ListPanel.transform, false);

            playerDetails.transform.Find("PlayerName")
                .gameObject.GetComponent<TextMeshProUGUI>()
                .text = playerName;

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
        catch (RelayServiceException)
        {
            lobbyExceptionCanvas.transform.Find("LobbyExceptionManager").GetComponent<LobbyExceptionManager>().SetErrorMessage(
                "Poor internet connection. Please try again!"
            );
            lobbyExceptionCanvas.SetActive(true);
            throw;
        }
        catch (Exception)
        {
            lobbyExceptionCanvas.transform.Find("LobbyExceptionManager").GetComponent<LobbyExceptionManager>().SetErrorMessage(
                "No internet connection!"
            );
            lobbyExceptionCanvas.SetActive(true);
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
        catch (RelayServiceException)
        {
            lobbyExceptionCanvas.transform.Find("LobbyExceptionManager").GetComponent<LobbyExceptionManager>().SetErrorMessage(
                "Poor internet connection!"
            );
            lobbyExceptionCanvas.SetActive(true);
        }
        catch (Exception)
        {
            lobbyExceptionCanvas.transform.Find("LobbyExceptionManager").GetComponent<LobbyExceptionManager>().SetErrorMessage(
                "No internet conenction!"
            );
            lobbyExceptionCanvas.SetActive(true);
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
                    loadingScreenCanvas.GetComponent<LoadingScreenManager>().SetProgress(1, currentNumberOfPlayers);
                    loadingScreenCanvas.SetActive(true);

                    await JoinRelay(lobby.Data["RelayCode"].Value);
                    SceneManager.LoadScene("Game");
                }
            }
        }
        catch (LobbyServiceException)
        {
            lobbyExceptionCanvas.transform.Find("LobbyExceptionManager").GetComponent<LobbyExceptionManager>().SetErrorMessage(
                "Poor internet connection. Please try again!"
            );
            lobbyExceptionCanvas.SetActive(true);
        }
        catch (Exception)
        {
            lobbyExceptionCanvas.transform.Find("LobbyExceptionManager").GetComponent<LobbyExceptionManager>().SetErrorMessage(
                "No internet conenction!"
            );
            lobbyExceptionCanvas.SetActive(true);
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
            lobbyExceptionCanvas.transform.Find("LobbyExceptionManager").GetComponent<LobbyExceptionManager>().SetErrorMessage(
                "Poor internet connection. Please try again!"
            );
            lobbyExceptionCanvas.SetActive(true);
        }
        catch (Exception)
        {
            lobbyExceptionCanvas.transform.Find("LobbyExceptionManager").GetComponent<LobbyExceptionManager>().SetErrorMessage(
                "No internet conenction!"
            );
            lobbyExceptionCanvas.SetActive(true);
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

    private async Task<bool> LeaveLobby()
    {
        try
        {
            string playerId = AuthenticationService.Instance.PlayerId;
            string lobbyId = lobby.Id;
            if (isHost)
            {
                await Lobbies.Instance.DeleteLobbyAsync(lobbyId);
            }
            else
            {
                await LobbyService.Instance.RemovePlayerAsync(lobbyId, playerId);
            }

            return true;
        }
        catch (LobbyServiceException)
        {
            lobbyExceptionCanvas.transform.Find("LobbyExceptionManager").GetComponent<LobbyExceptionManager>().SetErrorMessage(
                "Poor internet connection. Please try again!"
                );
            lobbyExceptionCanvas.SetActive(true);
            return false;
        }
        catch (Exception)
        {
            lobbyExceptionCanvas.transform.Find("LobbyExceptionManager").GetComponent<LobbyExceptionManager>().SetErrorMessage(
                "No internet connection!"
                );
            lobbyExceptionCanvas.SetActive(true);
            return false;
        }
    }

    private void GetKickedOut()
    {
        string playerId = AuthenticationService.Instance.PlayerId;

        if (!lobby.Players.Select(p => p.Id).Contains(playerId))
        {
            kickCanvas.SetActive(true);
        }
    }
}
