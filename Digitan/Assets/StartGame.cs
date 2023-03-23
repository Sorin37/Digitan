using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class StartGame : NetworkBehaviour
{
    [SerializeField] private GameObject gameGridPrefab;
    private GameObject gameGrid;

    public Dictionary<string, List<string>> resourcesDict;
    public Dictionary<string, int> playerHand;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        InitializeResourceDict();
        InitializeResourceDict();
    }



    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;
        if (Input.GetKeyUp(KeyCode.W))
        {
            FirstStepTowardsSuccessServerRpc();
        }
    }

    [ServerRpc]
    public void FirstStepTowardsSuccessServerRpc()
    {
        gameGrid = Instantiate(gameGridPrefab);
        gameGrid.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc]
    public void SecondServerRpc()
    {
        gameGrid.GetComponent<GameGrid>().CreateGrid();
    }

    [ClientRpc]
    public void PrintClientRpc(string msg)
    {
        print(msg);
    }

    private void InitializeResourceDict()
    {
        resourcesDict = new Dictionary<String, List<String>>();
    }

    void InitializePlayerHand()
    {
        playerHand = new Dictionary<string, int>();
        playerHand["Brick Resource"] = 0;
        playerHand["Grain Resource"] = 0;
        playerHand["Lumber Resource"] = 0;
        playerHand["Ore Resource"] = 0;
        playerHand["Wool Resource"] = 0;
    }

}
