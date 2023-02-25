using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyDetails : MonoBehaviour
{
    public Lobby lobby;
    // Start is called before the first frame update
    void Start()
    {
        tag = "Lobby";
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
