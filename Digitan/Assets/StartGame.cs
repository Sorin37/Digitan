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
    [SerializeField] private GameObject numbersGridPrefab;
    private GameObject gameGrid;
    private GameObject numbersGrid;

    public Dictionary<string, List<string>> resourcesDict;
    public Dictionary<string, int> playerHand;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsOwner)
            return;
        print("start");
        DontDestroyOnLoad(gameObject);
        InitializeResourceDict();
        InitializePlayerHand();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [ServerRpc]
    public void FirstStepTowardsSuccessServerRpc()
    {
        //gameGrid = Instantiate(gameGridPrefab);
        //gameGrid.name = "GameGrid";
        //gameGrid.GetComponent<NetworkObject>().Spawn();
        //gameGrid.GetComponent<GameGrid>().CreateGrid();
    }

    [ServerRpc]
    public void SecondServerRpc()
    {
        //numbersGrid = Instantiate(numbersGridPrefab);
        //gameGrid.name = "NumbersGrid";
        //numbersGrid.GetComponent<NetworkObject>().Spawn();
        //numbersGrid.GetComponent<NumbersGrid>().CreateGrid(gameGrid);
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

    private void InitializePlayerHand()
    {
        playerHand = new Dictionary<string, int>();
        playerHand["Brick Resource"] = 0;
        playerHand["Grain Resource"] = 0;
        playerHand["Lumber Resource"] = 0;
        playerHand["Ore Resource"] = 0;
        playerHand["Wool Resource"] = 0;
    }

}
