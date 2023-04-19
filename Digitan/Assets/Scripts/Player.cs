using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject gameGridPrefab;
    [SerializeField] private GameObject numbersGridPrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject settlementPrefab;
    [SerializeField] private GameObject cityPrefab;

    private GameObject gameGrid;
    private GameObject roadGrid;
    private GameObject settlementGrid;

    public NetworkVariable<FixedString64Bytes> resourcesCode = new NetworkVariable<FixedString64Bytes>("Uninitialized");
    public NetworkVariable<FixedString64Bytes> numbersCode = new NetworkVariable<FixedString64Bytes>("Uninitialized");
    public NetworkVariable<int> currentNrOfPlayers = new NetworkVariable<int>(0);
    public NetworkVariable<FixedString64Bytes> nickName = new NetworkVariable<FixedString64Bytes>("Uninitialized", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> order = new NetworkVariable<int>(1);
    public NetworkVariable<int> currentPlayerTurn = new NetworkVariable<int>(-1);

    public Dictionary<string, List<string>> resourcesDict;
    public Dictionary<string, int> playerHand;
    public Dictionary<string, bool> tradeDict;

    public int nrOfMaxPlayers;
    public int nrOfDeclinedTrades = 0;
    public Color color;

    public event EventHandler OnPlayersJoined;
    public event EventHandler<OnRoundEndEventArgs> OnRoundEnd;

    // Awake is called before all the Starts in a random order
    void Awake()
    {
        gameGrid = GameObject.Find("GameGrid");
        roadGrid = GameObject.Find("AvailableRoadsGrid");
        settlementGrid = GameObject.Find("AvailableSettlementGrid");
    }

    // OnNetworkSpawn is called before Start on all the clients
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        color = IdToColor(NetworkManager.Singleton.LocalClientId);
        currentPlayerTurn.Value = -1;
    }

    async void Start()
    {
        InitializeResourceDict();
        InitializePlayerHand();
        InitializeTradeDict();

        if (IsOwner)
        {
            OnRoundEnd += EndTurnEvent;
        }

        if (IsOwnedByServer)
        {


            //this is where you should do normal initialisations
            DontDestroyOnLoad(gameObject);

            if (IsHost)
            {
                resourcesCode.Value = GenerateResourcesCode();
            }

            gameGrid.GetComponent<GameGrid>().CreateGrid(resourcesCode.Value.ToString());
        }

        if (IsOwner)
        {
            await PopInformationFromLobby();
        }

        if (IsOwnedByServer)
        {
            PlayerJoinedServerRpc();
        }
    }

    private void InitializeTradeDict()
    {
        tradeDict = new Dictionary<string, bool>();
        tradeDict["Brick"] = false;
        tradeDict["Grain"] = false;
        tradeDict["Lumber"] = false;
        tradeDict["Ore"] = false;
        tradeDict["Wool"] = false;
        tradeDict["3to1"] = false;
    }

    private void EndTurnEvent(object sender, OnRoundEndEventArgs e)
    {
        print(e.diceRoll);

        if (e.diceRoll == 7)
            return;

        var player = GetMyPlayer().GetComponent<Player>();
        var resourcesDict = player.resourcesDict;
        var playerHand = player.playerHand;


        foreach (string resource in resourcesDict[e.diceRoll.ToString()])
        {
            playerHand[resource]++;
        }

        UpdateHand();
    }


    // Update is called once per frame
    void Update()
    {

    }

    private void InitializeResourceDict()
    {
        resourcesDict = new Dictionary<String, List<String>>();
        resourcesDict["2"] = new List<String>();
        resourcesDict["3"] = new List<String>();
        resourcesDict["4"] = new List<String>();
        resourcesDict["5"] = new List<String>();
        resourcesDict["6"] = new List<String>();
        resourcesDict["8"] = new List<String>();
        resourcesDict["9"] = new List<String>();
        resourcesDict["10"] = new List<String>();
        resourcesDict["11"] = new List<String>();
        resourcesDict["12"] = new List<String>();
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

    //private void printResInfo()
    //{
    //    foreach (var row in resourcesPrototype.Value)
    //    {
    //        List<string> list = new List<string>(row);
    //        print(String.Join(", ", list));
    //    }
    //}
    private string GenerateResourcesCode()
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

    public bool GetIsServer()
    {
        return IsServer;
    }

    public bool GetIsHost()
    {
        return IsHost;
    }

    public bool GetIsClient()
    {
        return IsClient;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlaceRoadServerRpc(int x, int y, Color color)
    {
        PlaceRoadClientRpc(x, y, color);
    }

    [ClientRpc]
    public void PlaceRoadClientRpc(int x, int y, Color color)
    {
        GameObject pressedCircle = roadGrid.GetComponent<RoadGrid>().roadGrid[x][y].gameObject;

        //create the model
        GameObject roadObject = Instantiate(roadPrefab,
                                            pressedCircle.transform.position,
                                            GetRotationFromPos((x, y)));

        //change the color
        roadObject.GetComponent<Renderer>().material.color = color;

        //neccessary piece of code so that the nearby circles know that it just got occupied
        roadObject.GetComponent<RoadCircle>().isOccupied = true;

        //destroy the road circle
        Destroy(pressedCircle);

        //make the road circles invisible
        Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Road Circle"));
    }

    private Quaternion GetRotationFromPos((int x, int y) pos)
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
    public void PlaceSettlementServerRpc(int x, int y, Color color)
    {
        PlaceSettlementClientRpc(x, y, color);
    }

    [ClientRpc]
    public void PlaceSettlementClientRpc(int x, int y, Color color)
    {
        GameObject pressedCircle = settlementGrid.GetComponent<SettlementGrid>().settlementGrid[x][y].gameObject;

        //get the nearby circles
        var colliders = Physics.OverlapSphere(
            pressedCircle.transform.position,
            2.5f,
            (int)(Mathf.Pow(2, LayerMask.NameToLayer("Unvisible Circle")) +
                Mathf.Pow(2, LayerMask.NameToLayer("Settlement Circle")))
            );

        //turn them visible
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.GetComponent<SettlementCircle>() != null)
            {
                colliders[i].gameObject.GetComponent<SettlementCircle>().isTooClose = true;
                colliders[i].gameObject.layer = LayerMask.NameToLayer("Unvisible Circle");
            }
        }

        //create the model
        GameObject settlementObject = Instantiate(
            settlementPrefab,
            pressedCircle.transform.position,
            Quaternion.Euler(90, 0, 0)
        );

        //so that the settlements do not disappear when other players want to place a city
        if (color != this.color)
        {
            settlementObject.layer = LayerMask.NameToLayer("My Settlement");
        }
        else
        {
            settlementObject.layer = LayerMask.NameToLayer("Settlement");
        }

        //change the color of the settlement
        settlementObject.GetComponent<Renderer>().material.color = color;

        //todo: change to settlement circle when getting a real settlement model
        // (since the current model has a road circle script attached to it)
        settlementObject.GetComponent<RoadCircle>().isOccupied = true;
        Destroy(pressedCircle);

        //make the settlement circles invisible
        Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Settlement Circle"));
    }

    private Color IdToColor(ulong id)
    {
        switch (id)
        {
            case 0ul:
                return new Color(0f, 0f, 200 / 255f);
            case 1ul:
                return Color.red;
            case 2ul:
                return new Color(1.0f, 0.64f, 0.0f);
            case 4ul:
                return Color.white;
            default:
                return Color.magenta;
        }
    }

    private GameObject GetMyPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().IsOwner)
                return p;
        }

        return null;
    }

    private async Task PopInformationFromLobby()
    {
        var lobbyGo = GameObject.FindGameObjectsWithTag("Lobby")[0];

        Lobby lobby;

        if (lobbyGo.GetComponent<HostLobby>())
        {
            lobby = GameObject.FindGameObjectsWithTag("Lobby")[0].GetComponent<HostLobby>().lobby;
        }
        else
        {
            lobby = GameObject.FindGameObjectsWithTag("Lobby")[0].GetComponent<LobbyDetails>().lobby;
        }

        lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);

        nrOfMaxPlayers = lobby.Players.Count;

        foreach (Unity.Services.Lobbies.Models.Player player in lobby.Players)
        {
            if (player.Id == AuthenticationService.Instance.PlayerId)
            {
                nickName.Value = player.Data["PlayerName"].Value;
                break;
            }
        }

        Destroy(lobbyGo);
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerJoinedServerRpc()
    {
        currentNrOfPlayers.Value++;
        print("Another one joi " + nrOfMaxPlayers + " ned" + currentNrOfPlayers.Value);

        OnPlayersJoined -= PlayersJoinedEvent;
        OnPlayersJoined += PlayersJoinedEvent;

        if (nrOfMaxPlayers == currentNrOfPlayers.Value)
        {
            OnPlayersJoined?.Invoke(this, EventArgs.Empty);
        }
    }

    private void PlayersJoinedEvent(object s, EventArgs e)
    {
        ChangeCurrentPlayerDetailsNameClientRpc(GetHostPlayer().GetComponent<Player>().nickName.Value.ToString());
        StartPlacingClientRpc(new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { 0 } }
        });
    }

    private void TurnCloseRoadsAvailable()
    {
        var colliders = Physics.OverlapSphere(
            transform.position,
            1,
           (int)Mathf.Pow(2, LayerMask.NameToLayer("Unvisible Circle"))
        );

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].gameObject.layer = LayerMask.NameToLayer("Road Circle");
        }
    }

    [ClientRpc]
    public void StartPlacingClientRpc(ClientRpcParams clientRpcParams)
    {
        Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Settlement Circle"));
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlacedServerRpc()
    {
        var player = GetHostPlayer().GetComponent<Player>();

        if ((int)OwnerClientId + player.order.Value == player.nrOfMaxPlayers)
        {
            player.order.Value = -1;
            ChangeCurrentPlayerDetailsColorClientRpc(OwnerClientId);
            ChangeCurrentPlayerDetailsNameClientRpc(GetPlayerWithId(OwnerClientId).GetComponent<Player>().nickName.Value.ToString());
            StartPlacingClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { OwnerClientId } }
            });
            return;
        }

        if ((int)OwnerClientId + player.order.Value == 0 || ((int)OwnerClientId + player.order.Value < 1))
        {
            Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Settlement Circle"));
            settlementGrid.GetComponent<SettlementGrid>().endStartPhase = true;
            ChangeCurrentPlayerDetailsColorClientRpc(0);
            ChangeCurrentPlayerDetailsNameClientRpc(GetHostPlayer().GetComponent<Player>().nickName.Value.ToString());
        }

        ChangeCurrentPlayerDetailsColorClientRpc((ulong)((int)OwnerClientId + player.order.Value));
        ChangeCurrentPlayerDetailsNameClientRpc(GetPlayerWithId((ulong)((int)OwnerClientId + player.order.Value)).GetComponent<Player>().nickName.Value.ToString());
        StartPlacingClientRpc(new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { (ulong)((int)OwnerClientId + player.order.Value) } }
        });
    }

    private GameObject GetHostPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().IsOwnedByServer)
                return p;
        }

        return null;
    }

    private GameObject GetPlayerWithId(ulong id)
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().OwnerClientId == id)
                return p;
        }

        return null;
    }

    [ClientRpc]
    public void EndStartPhaseClientRpc()
    {
        GetHostPlayer().GetComponent<Player>().settlementGrid.GetComponent<SettlementGrid>().isStartPhase = false;
        GetHostPlayer().GetComponent<Player>().settlementGrid.GetComponent<SettlementGrid>().TurnSettlementsInvisible();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PassTurnServerRpc()
    {
        currentPlayerTurn.Value = (currentPlayerTurn.Value + 1) % nrOfMaxPlayers;
        int dice1 = UnityEngine.Random.Range(1, 7);
        int dice2 = UnityEngine.Random.Range(1, 7);
        ChangeCurrentPlayerDetailsColorClientRpc((ulong)currentPlayerTurn.Value);
        ChangeCurrentPlayerDetailsNameClientRpc(
            GetPlayerWithId((ulong)currentPlayerTurn.Value)
            .GetComponent<Player>().nickName.Value.ToString()
        );
        PassTurnClientRpc(dice1 + dice2);
    }

    public class OnRoundEndEventArgs : EventArgs
    {
        public int diceRoll;
    }

    [ClientRpc]
    public void PassTurnClientRpc(int diceRoll)
    {
        GetMyPlayer().GetComponent<Player>().OnRoundEnd?.Invoke(this, new OnRoundEndEventArgs { diceRoll = diceRoll });
    }

    public void UpdateHand()
    {
        var playerHand = GetMyPlayer().GetComponent<Player>().playerHand;

        foreach (var card in playerHand)
        {
            GameObject.Find(card.Key.Substring(0, card.Key.IndexOf(" ")) + "Label").GetComponent<TextMeshProUGUI>().SetText("x " + card.Value.ToString());
        }
    }

    [ClientRpc]
    private void ChangeCurrentPlayerDetailsColorClientRpc(ulong id)
    {
        GameObject.Find("ColorPanel").GetComponent<Image>().color = IdToColor(id);
    }

    [ClientRpc]
    private void ChangeCurrentPlayerDetailsNameClientRpc(string name)
    {
        GameObject.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = name;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DisplayTradeOfferServerRpc(ulong tradeMakerId, int giveBrick, int giveGrain, int giveLumber, int giveOre, int giveWool, int getBrick, int getGrain, int getLumber, int getOre, int getWool)
    {
        List<ulong> targetClientIds = new List<ulong>();

        for (ulong i = 0; i < (ulong)nrOfMaxPlayers; i++)
        {
            if (i != tradeMakerId)
            {
                targetClientIds.Add(i);
            }
        }

        DisplayTradeOfferClientRpc(
            tradeMakerId,
            giveBrick, giveGrain, giveLumber, giveOre, giveWool,
            getBrick, getGrain, getLumber, getOre, getWool,
            new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = targetClientIds } }
        );
    }

    [ClientRpc]
    public void DisplayTradeOfferClientRpc(
        ulong tradeMakerId,
        int giveBrick, int giveGrain, int giveLumber, int giveOre, int giveWool,
        int getBrick, int getGrain, int getLumber, int getOre, int getWool,
        ClientRpcParams clientRpcParams)
    {
        var tradeOfferManager = Resources.FindObjectsOfTypeAll<TradeOfferManager>()[0];
        var giveDict = tradeOfferManager.giveDict;
        var getDict = tradeOfferManager.getDict;

        giveDict["Brick"] = getBrick;
        giveDict["Grain"] = getGrain;
        giveDict["Lumber"] = getLumber;
        giveDict["Ore"] = getOre;
        giveDict["Wool"] = getWool;

        getDict["Brick"] = giveBrick;
        getDict["Grain"] = giveGrain;
        getDict["Lumber"] = giveLumber;
        getDict["Ore"] = giveOre;
        getDict["Wool"] = giveWool;

        tradeOfferManager.tradeMakerId = tradeMakerId;
        tradeOfferManager.DrawDicts();
        tradeOfferManager.gameObject.transform.parent.gameObject.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AcceptTradeServerRpc(ulong tradeMakerId)
    {
        GetHostPlayer().GetComponent<Player>().CancelTradeClientRpc();
        GetHostPlayer().GetComponent<Player>().TradeAcceptedClientRpc(
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { tradeMakerId } }
            });
    }

    [ClientRpc]
    public void TradeAcceptedClientRpc(ClientRpcParams clientRpcParams)
    {
        var myPlayer = GetMyPlayer().GetComponent<Player>();
        var playerHand = myPlayer.playerHand;

        var tradeManager = Resources.FindObjectsOfTypeAll<TradeManager>()[0];

        tradeManager.gameObject.transform.parent.gameObject.SetActive(false);

        var giveDict = tradeManager.giveDict;
        var getDict = tradeManager.getDict;

        playerHand["Brick Resource"] += getDict["Brick"] - giveDict["Brick"];
        playerHand["Grain Resource"] += getDict["Grain"] - giveDict["Grain"];
        playerHand["Lumber Resource"] += getDict["Lumber"] - giveDict["Lumber"];
        playerHand["Ore Resource"] += getDict["Ore"] - giveDict["Ore"];
        playerHand["Wool Resource"] += getDict["Wool"] - giveDict["Wool"];

        myPlayer.UpdateHand();
    }

    [ServerRpc(RequireOwnership = false)]
    public void CancelTradeServerRpc()
    {
        GetHostPlayer().GetComponent<Player>().CancelTradeClientRpc();
    }

    [ClientRpc]
    public void CancelTradeClientRpc()
    {
        var tradeOfferManager = Resources.FindObjectsOfTypeAll<TradeOfferManager>()[0];
        tradeOfferManager.gameObject.transform.parent.gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddTradeDeclinedCountServerRpc(ulong tradeMakerId)
    {
        GetHostPlayer().GetComponent<Player>().AddTradeDeclinedCountClientRpc(
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new List<ulong> { tradeMakerId }
                }
            }
        );
    }

    [ClientRpc]
    public void AddTradeDeclinedCountClientRpc(ClientRpcParams clientRpcParams)
    {
        var hostPlayer = GetHostPlayer().GetComponent<Player>();
        var myPlayer = GetMyPlayer().GetComponent<Player>();

        print("I happened max :" + myPlayer.nrOfMaxPlayers + " nrOfDeclinedTrades: " + hostPlayer.nrOfDeclinedTrades);

        if (myPlayer.nrOfMaxPlayers - 1 == ++hostPlayer.nrOfDeclinedTrades)
        {
            var tradeManager = Resources.FindObjectsOfTypeAll<TradeManager>()[0];

            tradeManager.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    [ServerRpc(RequireOwnership =false)]
    public void PlaceCityServerRpc(float x, float y, float z, Color color)
    {
        PlaceCityClientRpc(x, y, z, color);
    }

    [ClientRpc]
    public void PlaceCityClientRpc(float x, float y, float z, Color color)
    {
        DestroyNearbySetllements();

        var city = Instantiate(
            cityPrefab, 
            new Vector3(x, y, z), 
            Quaternion.Euler(0, 0, 0)
        );

        //change the color
        city.GetComponent<Renderer>().material.color = color;
    }

    private void DestroyNearbySetllements()
    {
        var colliders = Physics.OverlapSphere(
            transform.position,
            1,
           (int)Mathf.Pow(2, LayerMask.NameToLayer("Settlement")) +
           (int)Mathf.Pow(2, LayerMask.NameToLayer("My Settlement"))
        );

        foreach (var collider in colliders)
        {
            Destroy(collider.gameObject);
        }
    }

}
