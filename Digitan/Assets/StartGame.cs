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

    public NetworkList<int> resourcesPrototype;

    public Dictionary<string, List<string>> resourcesDict;
    public Dictionary<string, int> playerHand;

    void Awake()
    {
        print("Awake");
        if(resourcesPrototype == null)
        {
            resourcesPrototype = new NetworkList<int>();
        }
    }

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost) {
            resourcesPrototype.Add(1); //if you want to initialize the list with some default values, this is a good time to do so.
            print(OwnerClientId.ToString() + resourcesPrototype[0]);
        }
    }

    void Start()
    {
        print("start" + IsOwner);

        DontDestroyOnLoad(gameObject);
        InitializeResourceDict();
        InitializePlayerHand();
        if (IsHost)
        {
            //initializeResourcesInfo();
            //printResInfo();
            print("Sunt host duh");

        }
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

    //private void initializeResourcesInfo()
    //{
    //    resourcesPrototype.Value = new string[5][];
    //    resourcesPrototype.Value[0] = new string[3];
    //    resourcesPrototype.Value[1] = new string[4];
    //    resourcesPrototype.Value[2] = new string[5];
    //    resourcesPrototype.Value[3] = new string[4];
    //    resourcesPrototype.Value[4] = new string[3];
    //}

    //private void printResInfo()
    //{
    //    foreach (var row in resourcesPrototype.Value)
    //    {
    //        List<string> list = new List<string>(row);
    //        print(String.Join(", ", list));
    //    }
    //}

}
