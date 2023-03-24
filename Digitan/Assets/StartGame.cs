using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Assertions;
using static Unity.Collections.AllocatorManager;

public class StartGame : NetworkBehaviour
{
    [SerializeField] private GameObject gameGridPrefab;
    [SerializeField] private GameObject numbersGridPrefab;
    private GameObject gameGrid;
    private GameObject numbersGrid;

    public NetworkVariable<FixedString64Bytes> resourcesPrototype = new NetworkVariable<FixedString64Bytes>();

    public Dictionary<string, List<string>> resourcesDict;
    public Dictionary<string, int> playerHand;

    void Awake()
    {
        //print("Awake");
    }

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost)
        {
            print("Am generat cod");
            resourcesPrototype.Value = generateResourcesCode();
        }

        print("ASta e codul pe care il am acum");
        print(OwnerClientId.ToString() + " " + resourcesPrototype.Value);
    }

    void Start()
    {
        if (!IsOwner)
            return;

        DontDestroyOnLoad(gameObject);
        InitializeResourceDict();
        InitializePlayerHand();

        print("ASta e codul cu care fac trb");
        print(OwnerClientId.ToString() + " " + resourcesPrototype.Value);

        gameGrid = Instantiate(gameGridPrefab);
        gameGrid.name = "GameGrid";
        gameGrid.GetComponent<GameGrid>().CreateGrid(resourcesPrototype.Value.ToString());
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
    private string generateResourcesCode()
    {
        string code = "";

        List<string> hexPool = new List<string> {
            "b", "b", "b",
            "d",
            "g", "g", "g", "g",
            "l", "l", "l", "l",
            "o", "o", "o",
            "w", "w", "w", "w"
        };
        List<int> lengths = new List<int>() { 3, 4, 5, 4, 3 };
        for (int i = 0; i < lengths.Count; i++)
        {
            code += i;
            for (int j = 0; j < lengths[i]; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0, hexPool.Count);
                string letter = hexPool[randomIndex];
                hexPool.RemoveAt(randomIndex);
                code += letter;
            }
        }

        return code;
    }

}
