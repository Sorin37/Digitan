using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class ClientDetails : MonoBehaviour
{
    [SerializeField] private Button connectButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_InputField LobbyNameInput;
    [SerializeField] private TMP_InputField NicknameInput;
    [SerializeField] private Canvas popupCanvas;
    [SerializeField] private Canvas inputCanvas;
    [SerializeField] private GameObject Lobby;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        InitConnectButton();
        InitBackButton();
    }

    private void InitConnectButton()
    {
        connectButton.onClick.AddListener(async () =>
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(
                new QueryLobbiesOptions
                {
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter(
                            QueryFilter.FieldOptions.Name,
                            LobbyNameInput.text,
                            QueryFilter.OpOptions.EQ
                        )
                    }
                }
            );

            if (queryResponse.Results.Count == 0)
            {
                inputCanvas.gameObject.SetActive(false);
                popupCanvas.gameObject.SetActive(true);
                return;
            }

            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
            {
                Player = new Unity.Services.Lobbies.Models.Player
                {
                    Data = new Dictionary<string, PlayerDataObject> {
                    {
                        "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, NicknameInput.text) }
                    }
                }
            };

            Lobby.GetComponent<LobbyDetails>().lobby = await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id, options);

            SceneManager.LoadScene("Lobby");
        });
    }

    private void InitBackButton()
    {
        backButton.onClick.AddListener(() =>
        {
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

    }

    // Update is called once per frame
    void Update()
    {
    }
}
