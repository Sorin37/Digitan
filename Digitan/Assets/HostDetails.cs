using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class HostDetails : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private GameObject hostLobby;

    private void Awake()
    {
        hostButton.interactable = false;

        hostButton.onClick.AddListener(async () =>
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
            Debug.Log( queryResponse.Results.Count.ToString() );
            //tell the mdfk that this name is already taken

            //put to sleep because we use a free API
            System.Threading.Thread.Sleep(1000);

            await hostLobby.GetComponent<HostLobby>().CreateLobby(nameInput.text);

            SceneManager.LoadScene("Lobby");
        });

        hostLobby.GetComponent<HostLobby>().InitializeUnityServices();
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
