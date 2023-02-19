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
        System.Threading.Thread.Sleep(1000);

        QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(
                new QueryLobbiesOptions
                {
                    Filters = new List<QueryFilter>
                    {
                        new QueryFilter(
                            QueryFilter.FieldOptions.Name,
                            "ASD",
                            QueryFilter.OpOptions.EQ
                        )
                    }
                }
            );

        foreach (Player player in queryResponse.Results[0].Players)
        {
            Debug.LogError(player.Id);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
