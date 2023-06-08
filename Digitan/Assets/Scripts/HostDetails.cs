using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;
using Input = UnityEngine.Input;

public class HostDetails : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button backButton;
    [SerializeField] public TMP_InputField LobbyNameInput;
    [SerializeField] public TMP_InputField NicknameInput;
    [SerializeField] private GameObject hostLobby;
    [SerializeField] private Canvas popupCanvas;
    [SerializeField] private Canvas inputCanvas;
    private int SelectedInput;

    private void Awake()
    {
        hostButton.interactable = false;

        InitHostButton();
        InitBackButton();

        hostLobby.GetComponent<HostLobby>().InitializeUnityServices();
    }

    private void InitHostButton()
    {
        hostButton.onClick.AddListener(async () =>
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

            if (queryResponse.Results.Count > 0)
            {
                inputCanvas.gameObject.SetActive(false);
                popupCanvas.gameObject.SetActive(true);
                return;
            }

            //put to sleep because we use a free API
            System.Threading.Thread.Sleep(1000);

            await hostLobby.GetComponent<HostLobby>().CreateLobby(LobbyNameInput.text, NicknameInput.text);

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
            hostButton.Select();
        }
    }

    void SelectInputField(int selectedInputNumber)
    {
        switch(selectedInputNumber)
        {
            case 0: LobbyNameInput.Select();
                break;
            case 1: NicknameInput.Select(); 
                break;
        }
    }

    public void LobbyNameSelected() => SelectedInput = 0;
    public void NicknameSelected() => SelectedInput = 1;
}
