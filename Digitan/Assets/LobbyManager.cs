using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.EventSystems;

public class LobbyManager : MonoBehaviour
{

    // Start is called before the first frame update
    async void Start()
    {
        QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

        Debug.Log("Lobbies found: " + queryResponse.Results.Count);

        foreach(Lobby lobby in queryResponse.Results)
        {
            Debug.Log(lobby.Name);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
