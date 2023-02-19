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
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Canvas popupCanvas;
    [SerializeField] private Canvas inputCanvas;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        connectButton.onClick.AddListener(async () =>
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(
                new QueryLobbiesOptions
                {
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter(
                            QueryFilter.FieldOptions.Name,
                            nameInput.text,
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

            await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);

            SceneManager.LoadScene("Lobby");
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
