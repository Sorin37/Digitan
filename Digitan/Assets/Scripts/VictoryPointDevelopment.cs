using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryPointDevelopment : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetHostPlayer().AddVictoryPointServerRpc(new Unity.Netcode.ServerRpcParams());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Player GetHostPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().IsOwnedByServer)
                return p.GetComponent<Player>();
        }

        return null;
    }
}
