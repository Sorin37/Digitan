using System.Collections;
using System.Collections.Generic;
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

        hostButton.onClick.AddListener(() =>
        {
            hostLobby.GetComponent<HostLobby>().CreateLobby(nameInput.text);
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
