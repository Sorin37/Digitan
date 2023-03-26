using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject gameGridPrefab;
    [SerializeField] private GameObject numbersGridPrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject settlementPrefab;
    private GameObject gameGrid;
    private GameObject roadGrid;
    private GameObject settlementGrid;

    public NetworkVariable<FixedString64Bytes> resourcesCode = new NetworkVariable<FixedString64Bytes>("Uninitialized");
    public NetworkVariable<FixedString64Bytes> numbersCode = new NetworkVariable<FixedString64Bytes>("Uninitialized");

    public Dictionary<string, List<string>> resourcesDict;
    public Dictionary<string, int> playerHand;

    // Awake is called before all the Starts in a random order
    void Awake()
    {
        gameGrid = GameObject.Find("GameGrid");
        roadGrid = GameObject.Find("AvailableRoadsGrid");
        settlementGrid = GameObject.Find("AvailableSettlementGrid");
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
        print(msg);
    }

    [ServerRpc(RequireOwnership = false)]
    public void placeRoadServerRpc(int x, int y)
    {
        placeRoadClientRpc(x, y);
    }

    [ClientRpc]
    public void placeRoadClientRpc(int x, int y)
    {
        GameObject pressedCircle = roadGrid.GetComponent<RoadGrid>().roadGrid[x][y].gameObject;

        var colliders = Physics.OverlapSphere(
                pressedCircle.transform.position,
                2,
               (int)Mathf.Pow(2, LayerMask.NameToLayer("Unvisible Circle"))
            );


        //turn the nearby road circles visible
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.GetComponent<SettlementCircle>() != null)
            {
                if (!colliders[i].gameObject.GetComponent<SettlementCircle>().isTooClose)
                {
                    colliders[i].gameObject.layer = LayerMask.NameToLayer("Settlement Circle");
                }
            }
            else
            {
                colliders[i].gameObject.layer = LayerMask.NameToLayer("Road Circle");
            }
        }


        //create the model
        GameObject roadObject = Instantiate(roadPrefab,
                                            pressedCircle.transform.position,
                                            getRotationFromPos((x, y)));

        //neccessary piece of code so that the nearby circles know that it just got occupied
        roadObject.GetComponent<RoadCircle>().isOccupied = true;

        //destroy the road circle
        Destroy(pressedCircle);

        //make the road circles invisible
        Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Road Circle"));
    }

    private Quaternion getRotationFromPos((int x, int y) pos)
    {
        int y = 0;

        if (pos.x % 2 == 0)
        {
            y = 60;

            if (pos.y % 2 == 1)
            {
                y *= -1;
            }
        }

        if (pos.x % 2 == 0 && pos.x > 5)
        {
            y = -60;

            if (pos.y % 2 == 1)
            {
                y *= -1;
            }
        }

        return Quaternion.Euler(-90, y, 0);
    }

    [ServerRpc(RequireOwnership = false)]
    public void placeSettlementServerRpc(int x, int y)
    {
        placeSettlementClientRpc(x, y);
    }

    [ClientRpc]
    public void placeSettlementClientRpc(int x, int y)
    {
        print("I be placing");

        GameObject pressedCircle = settlementGrid.GetComponent<SettlementGrid>().settlementGrid[x][y].gameObject;

        var colliders = Physics.OverlapSphere(
            pressedCircle.transform.position,
            2.5f,
            (int)(Mathf.Pow(2, LayerMask.NameToLayer("Unvisible Circle")) +
                Mathf.Pow(2, LayerMask.NameToLayer("Settlement Circle")))
            );

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.GetComponent<SettlementCircle>() != null)
            {
                colliders[i].gameObject.GetComponent<SettlementCircle>().isTooClose = true;
                colliders[i].gameObject.layer = LayerMask.NameToLayer("Unvisible Circle");
            }
        }

        GameObject settlementObject = Instantiate(
            settlementPrefab,
            pressedCircle.transform.position,
            Quaternion.Euler(90, 0, 0)
        );

        //todo: change to settlement circle when getting a real settlement model
        // (since the current model has a road circle script attached to it)
        settlementObject.GetComponent<RoadCircle>().isOccupied = true;
        Destroy(pressedCircle);

        Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Settlement Circle"));
    }
}
