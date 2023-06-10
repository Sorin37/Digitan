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
using System;

public class ClientDetails : MonoBehaviour
{
    [SerializeField] private Button connectButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_InputField LobbyNameInput;
    [SerializeField] private TMP_InputField NicknameInput;
    [SerializeField] private GameObject popupCanvas;
    [SerializeField] private GameObject Lobby;
    [SerializeField] private GameObject lobbyExceptionCanvas;

    private int SelectedInput;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();

        try
        {
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
        catch (Exception)
        {
            lobbyExceptionCanvas.transform.Find("LobbyExceptionManager").GetComponent<LobbyExceptionManager>().SetErrorMessage(
                "No internet connection!"
                );
            lobbyExceptionCanvas.SetActive(true);
            return;
        }

        InitConnectButton();
        InitBackButton();
    }

    private void InitConnectButton()
    {
        connectButton.onClick.AddListener(async () =>
        {
            if (InputsAreEmpty())
            {
                popupCanvas.transform.Find("PopupManager").GetComponent<PopupHostManager>().SetErrorMessage(
                    "You can not leave the fields empty!"
                    );
                popupCanvas.SetActive(true);
                return;
            }

            if (InputsAreTooBig(30))
            {
                popupCanvas.transform.Find("PopupManager").GetComponent<PopupHostManager>().SetErrorMessage(
                    "The introduced words are too long!"
                    );
                popupCanvas.SetActive(true);
                return;
            }

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
                popupCanvas.transform.Find("PopupManager").GetComponent<PopupHostManager>().SetErrorMessage(
                    "No lobby with such name!"
                    );
                popupCanvas.SetActive(true);
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
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            SelectedInput++;
            if (SelectedInput > 1)
                SelectedInput = 0;
            SelectInputField(SelectedInput);
        }

        if (Input.GetKeyUp(KeyCode.Return))
        {
            connectButton.Select();
        }
    }

    void SelectInputField(int selectedInputNumber)
    {
        switch (selectedInputNumber)
        {
            case 0:
                LobbyNameInput.Select();
                break;
            case 1:
                NicknameInput.Select();
                break;
        }
    }

    public void LobbyNameSelected() => SelectedInput = 0;
    public void NicknameSelected() => SelectedInput = 1;

    private bool InputsAreEmpty()
    {
        return string.IsNullOrEmpty(LobbyNameInput.text) || string.IsNullOrEmpty(NicknameInput.text);
    }

    private bool InputsAreTooBig(int maxLength)
    {
        return LobbyNameInput.text.Length > maxLength || NicknameInput.text.Length > maxLength;
    }
}
