using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class StartGame : NetworkBehaviour
{
    [SerializeField] private GameObject gameGridPrefab;
    [SerializeField] private GameObject numbersGridPrefab;
    private GameObject gameGrid;
    private GameObject numbersGrid;

    public NetworkVariable<FixedString64Bytes> resourcesCode = new NetworkVariable<FixedString64Bytes>("Uninitialized");
    public NetworkVariable<FixedString64Bytes> numbersCode = new NetworkVariable<FixedString64Bytes>("Uninitialized");

    public Dictionary<string, List<string>> resourcesDict;
    public Dictionary<string, int> playerHand;

    // Awake is called before all the Starts in a random order
    void Awake()
    {

    }

    // OnNetworkSpawn is called before Start
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }

    void Start()
    {
        if (!IsOwnedByServer)
            return;

        DontDestroyOnLoad(gameObject);
        InitializeResourceDict();
        InitializePlayerHand();

        if (IsHost)
        {
            resourcesCode.Value = generateResourcesCode();
        }

        gameGrid = GameObject.Find("GameGrid");
        gameGrid.GetComponent<GameGrid>().CreateGrid(resourcesCode.Value.ToString());
    }


    // Update is called once per frame
    void Update()
    {

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

    public bool getIsServer()
    {
        return IsServer;
    }

    public bool getIsHost()
    {
        return IsHost;
    }

    public bool getIsClient()
    {
        return IsClient;
    }

    [ClientRpc]
    public void printClientRpc(string msg)
    {
        if (!IsOwner)
            return;
        print(msg);
    }
}
