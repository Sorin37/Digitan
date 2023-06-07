using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using UnityEngine.UIElements;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject gameGridPrefab;
    [SerializeField] private GameObject numbersGridPrefab;
    [SerializeField] private GameObject roadPrefab;
    [SerializeField] private GameObject settlementPrefab;
    [SerializeField] private GameObject cityPrefab;

    [SerializeField] private GameObject thiefPrefab;

    [SerializeField] private GameObject chapelPrefab;
    [SerializeField] private GameObject greatHallPrefab;
    [SerializeField] private GameObject knightPrefab;
    [SerializeField] private GameObject libraryPrefab;
    [SerializeField] private GameObject marketPrefab;
    [SerializeField] private GameObject monopolyPrefab;
    [SerializeField] private GameObject roadBuildingPrefab;
    [SerializeField] private GameObject universityPrefab;
    [SerializeField] private GameObject yearOfPlentyPrefab;

    [SerializeField] private Sprite redDice1;
    [SerializeField] private Sprite redDice2;
    [SerializeField] private Sprite redDice3;
    [SerializeField] private Sprite redDice4;
    [SerializeField] private Sprite redDice5;
    [SerializeField] private Sprite redDice6;
    [SerializeField] private Sprite yellowDice1;
    [SerializeField] private Sprite yellowDice2;
    [SerializeField] private Sprite yellowDice3;
    [SerializeField] private Sprite yellowDice4;
    [SerializeField] private Sprite yellowDice5;
    [SerializeField] private Sprite yellowDice6;

    [SerializeField] private GameObject messagePrefab;

    private GameObject gameGrid;
    private GameObject roadGrid;
    private GameObject settlementGrid;

    public NetworkVariable<FixedString64Bytes> resourcesCode = new NetworkVariable<FixedString64Bytes>("Uninitialized");
    public NetworkVariable<FixedString64Bytes> numbersCode = new NetworkVariable<FixedString64Bytes>("Uninitialized");
    public NetworkVariable<int> currentNrOfPlayers = new NetworkVariable<int>(0);
    public NetworkVariable<FixedString64Bytes> nickName = new NetworkVariable<FixedString64Bytes>("Uninitialized", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> order = new NetworkVariable<int>(1);
    public NetworkVariable<int> currentPlayerTurn = new NetworkVariable<int>(-1);
    public NetworkVariable<int> nrOfFinishedDiscards = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> nrOfVictoryPoints = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> nrOfUsedKnights = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> hasLargestArmy = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> longestRoad = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<bool> hasLongestRoad = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    public Dictionary<string, List<string>> resourcesDict;
    public Dictionary<string, int> playerHand;
    public Dictionary<string, bool> tradeDict;

    public int nrOfMaxPlayers;
    public int nrOfDeclinedTrades = 0;
    public Color color;

    public List<string> developments = new List<string>();
    private List<string> developmentsDeck;

    public event EventHandler OnPlayersJoined;
    public event EventHandler OnFinishDiscardChanged;

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

        if (NetworkManager.Singleton.LocalClientId == 0)
        {
            OnFinishDiscardChanged -= FinishedDiscarding;
            OnFinishDiscardChanged += FinishedDiscarding;
        }
    }

    private void FinishedDiscarding(object sender, EventArgs e)
    {
        var player = GetHostPlayer();

        if (player.nrOfFinishedDiscards.Value == 0)
        {
            player.DiscardHandServerRpc();
        }

        if (player.nrOfFinishedDiscards.Value == player.nrOfMaxPlayers)
        {
            player.HideDiscardWaitingCanvasServerRpc();
        }
    }

    async void Start()
    {
        InitializeResourceDict();
        InitializePlayerHand();
        InitializeTradeDict();

        if (IsOwnedByServer)
        {
            //this is where you should do normal initialisations
            DontDestroyOnLoad(gameObject);

            if (IsHost)
            {
                resourcesCode.Value = GenerateResourcesCode();
                InitializeDevelopmentDeck();
            }

            gameGrid.GetComponent<GameGrid>().CreateGrid(resourcesCode.Value.ToString());
        }

        if (IsOwner)
        {
            await PopInformationFromLobby();
            nrOfVictoryPoints.OnValueChanged += VictoryPointsChangedEvent;
        }

        if (IsOwnedByServer)
        {
            PlayerJoinedServerRpc();
        }
    }

    private void VictoryPointsChangedEvent(int oldValue, int newValue)
    {
        print("Am schimbat punctele victorioase: " + newValue);

        if (newValue > 9)
        {
            GetHostPlayer().VictoryServerRpc(GetMyPlayer().nickName.Value.ToString());
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

        roadGrid.GetComponent<RoadGrid>().roadGrid[x][y] = roadObject;

        roadObject.name = x + " " + y + " Road";

        //change the color
        roadObject.GetComponent<Renderer>().material.color = color;
        roadObject.GetComponent<RoadDetails>().color = color;

        //neccessary piece of code so that the nearby circles know that it got occupied
        roadObject.GetComponent<RoadCircle>().isOccupied = true;

        //destroy the road circle
        Destroy(pressedCircle);

        //make the road circles invisible
        if (!roadGrid.GetComponent<RoadGrid>().usedRoadBuilding)
        {
            Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Road Circle"));
        }
        else
        {
            roadGrid.GetComponent<RoadGrid>().usedRoadBuilding = false;
        }

        CalculateLongestRoad();
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
    public void PlaceSettlementServerRpc(int x, int y, Color color, ServerRpcParams serverRpcParams)
    {
        GetPlayerWithId(serverRpcParams.Receive.SenderClientId).nrOfVictoryPoints.Value++;
        PlaceSettlementClientRpc(x, y, color, serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    public void PlaceSettlementClientRpc(int x, int y, Color color, ulong playerId)
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

        settlementObject.name = x + " " + y + " Settlement";

        settlementObject.GetComponent<SettlementPiece>().playerId = playerId;
        settlementObject.GetComponent<SettlementPiece>().color = color;

        //so that the settlements do not disappear when other players want to place a city
        if (color == this.color)
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

        CalculateLongestRoad();
    }

    public Color IdToColor(ulong id)
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

    public string IdToHexColor(ulong id)
    {
        switch (id)
        {
            case 0ul:
                return "#0000CC";
            case 1ul:
                return "#FF0000";
            case 2ul:
                return "#FFA300";
            case 4ul:
                return "#FFFFFF";
            default:
                return "#FF00FF";
        }
    }

    private Player GetMyPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().IsOwner)
                return p.GetComponent<Player>();
        }

        return null;
    }

    private async Task PopInformationFromLobby()
    {
        var lobbies = GameObject.FindGameObjectsWithTag("Lobby");

        GameObject lobbyManager = null;
        GameObject hostLobby = null;

        Lobby lobby = null;

        if (lobbies.Count() == 1)
        {
            lobbyManager = lobbies[0];
            lobby = lobbyManager.GetComponent<LobbyManager>().lobby;
        }
        else
        {
            foreach (var lob in lobbies)
            {
                if (lob.GetComponent<HostLobby>())
                {
                    hostLobby = lob;
                    lobby = hostLobby.GetComponent<HostLobby>().lobby;
                }
                else
                {
                    lobbyManager = lob;
                }
            }

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

        Destroy(lobbyManager.gameObject.transform.parent.gameObject);
        if (hostLobby != null)
            Destroy(hostLobby.gameObject);
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
        ChangeCurrentPlayerDetailsNameClientRpc(GetHostPlayer().nickName.Value.ToString());
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
        var player = GetHostPlayer();

        if ((int)OwnerClientId + player.order.Value == player.nrOfMaxPlayers)
        {
            player.order.Value = -1;
            ChangeCurrentPlayerDetailsColorClientRpc(OwnerClientId);
            ChangeCurrentPlayerDetailsNameClientRpc(GetPlayerWithId(OwnerClientId).nickName.Value.ToString());
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
            ChangeCurrentPlayerDetailsNameClientRpc(GetHostPlayer().nickName.Value.ToString());
        }

        ChangeCurrentPlayerDetailsColorClientRpc((ulong)((int)OwnerClientId + player.order.Value));
        ChangeCurrentPlayerDetailsNameClientRpc(GetPlayerWithId((ulong)((int)OwnerClientId + player.order.Value)).nickName.Value.ToString());
        StartPlacingClientRpc(new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { (ulong)((int)OwnerClientId + player.order.Value) } }
        });
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

    private Player GetPlayerWithId(ulong id)
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().OwnerClientId == id)
                return p.GetComponent<Player>();
        }

        return null;
    }

    [ClientRpc]
    public void EndStartPhaseClientRpc()
    {
        GetHostPlayer().settlementGrid.GetComponent<SettlementGrid>().isStartPhase = false;
        GetHostPlayer().settlementGrid.GetComponent<SettlementGrid>().TurnSettlementsInvisible();
    }

    [ServerRpc(RequireOwnership = false)]
    public void PassTurnServerRpc()
    {
        currentPlayerTurn.Value = (currentPlayerTurn.Value + 1) % nrOfMaxPlayers;
        ChangeCurrentPlayerDetailsColorClientRpc((ulong)currentPlayerTurn.Value);
        ChangeCurrentPlayerDetailsNameClientRpc(
            GetPlayerWithId((ulong)currentPlayerTurn.Value).nickName.Value.ToString()
        );
    }
    public void UpdateHand()
    {
        var playerHand = GetMyPlayer().playerHand;

        foreach (var card in playerHand)
        {
            var labelGO = GameObject.Find(card.Key.Substring(0, card.Key.IndexOf(" ")) + "Label");

            if (labelGO == null)
                continue;

            labelGO.GetComponent<TextMeshProUGUI>().SetText("x " + card.Value.ToString());
        }
    }

    [ClientRpc]
    private void ChangeCurrentPlayerDetailsColorClientRpc(ulong id)
    {
        GameObject.Find("ColorPanel").GetComponent<UnityEngine.UI.Image>().color = IdToColor(id);
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
        GetHostPlayer().CancelTradeClientRpc();
        GetHostPlayer().TradeAcceptedClientRpc(
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { tradeMakerId } }
            });
    }

    [ClientRpc]
    public void TradeAcceptedClientRpc(ClientRpcParams clientRpcParams)
    {
        var myPlayer = GetMyPlayer();
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
        GetHostPlayer().CancelTradeClientRpc();
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
        GetHostPlayer().AddTradeDeclinedCountClientRpc(
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
        var hostPlayer = GetHostPlayer();
        var myPlayer = GetMyPlayer();

        if (myPlayer.nrOfMaxPlayers - 1 == ++hostPlayer.nrOfDeclinedTrades)
        {
            var tradeManager = Resources.FindObjectsOfTypeAll<TradeManager>()[0];

            tradeManager.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlaceCityServerRpc(float x, float y, float z, Color color, ServerRpcParams serverRpcParams)
    {
        GetPlayerWithId(serverRpcParams.Receive.SenderClientId).nrOfVictoryPoints.Value++;
        PlaceCityClientRpc(x, y, z, color, serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    public void PlaceCityClientRpc(float x, float y, float z, Color color, ulong playerId)
    {
        var position = new Vector3(x, y, z);

        DestroyNearbySetllements(position);

        var city = Instantiate(
            cityPrefab,
            position,
            Quaternion.Euler(0, 0, 0)
        );

        city.name = Math.Round(x, 2) + " " + Math.Round(z, 2) + " City";

        city.GetComponent<CityPiece>().playerId = playerId;
        city.GetComponent<CityPiece>().color = color;

        //change the color
        foreach (var material in city.GetComponent<Renderer>().materials)
        {
            material.color = color;
        }
    }

    private void DestroyNearbySetllements(Vector3 position)
    {
        var colliders = Physics.OverlapSphere(
            position,
            1,
           (int)Mathf.Pow(2, LayerMask.NameToLayer("Settlement")) +
           (int)Mathf.Pow(2, LayerMask.NameToLayer("My Settlement"))
        );

        foreach (var collider in colliders)
        {
            Destroy(collider.gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RollDiceServerRpc()
    {
        int redDice = UnityEngine.Random.Range(1, 7);
        int yellowDice = UnityEngine.Random.Range(1, 7);

        GetHostPlayer().RollDiceClientRpc(redDice, yellowDice);

    }

    [ClientRpc]
    public void RollDiceClientRpc(int redDice, int yellowDice)
    {
        var player = GetHostPlayer();

        player.DisplayDiceServerRpc(redDice, yellowDice);

        if ((redDice + yellowDice) == 7)
        {
            if (IsServer)
            {
                player.ResetFinishedDiscardsServerRpc();
            }
            player.DisplayThiefCirclesServerRpc();
        }
        else
        {
            var myPlayer = GetMyPlayer();
            var resourcesDict = myPlayer.resourcesDict;
            var playerHand = myPlayer.playerHand;

            foreach (string resource in resourcesDict[(redDice + yellowDice).ToString()])
            {
                playerHand[resource]++;
            }

            UpdateHand();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void MoveThiefServerRpc(Vector3 newPosition)
    {
        MoveThiefClientRpc(newPosition);
    }

    [ClientRpc]
    public void MoveThiefClientRpc(Vector3 newPosition)
    {
        var thiefPiece = GameObject.Find("Thief");

        if (thiefPiece != null)
        {
            //display the circle the thief is taken from
            var colliders = Physics.OverlapSphere(
                thiefPiece.transform.position,
                1f,
                (int)Mathf.Pow(2, LayerMask.NameToLayer("Unvisible Thief Circle")));

            foreach (var collider in colliders)
            {
                collider.gameObject.layer = LayerMask.NameToLayer("Thief Circle");
            }

            thiefPiece.transform.position = new Vector3(newPosition.x, 0.75f, newPosition.z);

            //hide the circle the thief is placed on
            colliders = Physics.OverlapSphere(
                thiefPiece.transform.position,
                1f,
                (int)Mathf.Pow(2, LayerMask.NameToLayer("Thief Circle")));

            foreach (var collider in colliders)
            {
                collider.gameObject.layer = LayerMask.NameToLayer("Unvisible Thief Circle");
            }

        }
        else
        {
            var newThief = Instantiate(
                thiefPrefab,
                new Vector3(newPosition.x, 0.75f, newPosition.z),
                Quaternion.Euler(0, 0, 0));

            newThief.name = "Thief";


            //hide the circle the thief is placed on
            var colliders = Physics.OverlapSphere(
                newThief.transform.position,
                1f,
                (int)Mathf.Pow(2, LayerMask.NameToLayer("Thief Circle")));

            foreach (var collider in colliders)
            {
                collider.gameObject.layer = LayerMask.NameToLayer("Unvisible Thief Circle");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void StealServerRpc(ulong targetId, ServerRpcParams serverRpcParams)
    {
        StealClientRpc(
            serverRpcParams.Receive.SenderClientId,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { targetId } }
            }
        );
    }

    [ClientRpc]
    public void StealClientRpc(ulong sender, ClientRpcParams clientRpcParams)
    {
        var player = GetMyPlayer();
        var hand = player.playerHand;

        List<string> resources = new List<string>();

        foreach (var key in hand.Keys)
        {
            for (int i = 0; i < hand[key]; i++)
            {
                resources.Add(key);
            }
        }

        if (resources.Count == 0)
            return;

        int randomIndex = UnityEngine.Random.Range(0, resources.Count);
        var resource = resources[randomIndex];

        hand[resource]--;

        player.UpdateHand();

        GetHostPlayer().AddResourceServerRpc(
            resource,
            sender
        );
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddResourceServerRpc(string resource, ulong sender)
    {
        GetHostPlayer().AddResourceClientRpc(
            resource,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { sender } }
            }
        );
    }

    [ClientRpc]
    public void AddResourceClientRpc(string resource, ClientRpcParams clientRpcParams)
    {
        var player = GetMyPlayer();
        player.playerHand[resource]++;
        player.UpdateHand();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DiscardHandServerRpc()
    {
        var player = GetHostPlayer();
        player.DiscardHandClientRpc();
    }

    [ClientRpc]
    public void DiscardHandClientRpc()
    {

        int handSize = 0;
        var myPlayer = GetMyPlayer();
        var hostPlayer = GetHostPlayer();

        foreach (var count in myPlayer.playerHand.Values)
        {
            handSize += count;
        }

        if (handSize > 7)
        {
            Resources.FindObjectsOfTypeAll<DiscardManager>()[0].transform.parent.gameObject.SetActive(true);
        }
        else
        {
            if (IsOwnedByServer)
            {
                Resources.FindObjectsOfTypeAll<DiscardWaitingManager>()[0].transform.parent.gameObject.SetActive(true);
                hostPlayer.FinishedDiscardingServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void DisplayThiefCirclesServerRpc()
    {
        var player = GetHostPlayer();

        player.DisplayThiefCirclesClientRpc(new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { (ulong)player.currentPlayerTurn.Value } }
        });
    }

    [ClientRpc]
    public void DisplayThiefCirclesClientRpc(ClientRpcParams clientRpcParams)
    {
        Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Thief Circle"));
    }

    [ServerRpc(RequireOwnership = false)]
    public void ModifyResourceDictServerRpc(string action, string number, string resource, ulong playerId)
    {
        GetHostPlayer().ModifyResourceDictClientRpc(
            action,
            number,
            resource,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { playerId } }
            });
    }

    [ClientRpc]
    public void ModifyResourceDictClientRpc(string action, string number, string resource, ClientRpcParams clientRpcParams)
    {
        var resourceDict = GetMyPlayer().resourcesDict;

        if (action == "Block")
        {
            resourceDict[number].Remove(resource);
        }

        if (action == "Free")
        {
            resourceDict[resource].Add(resource);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void FinishedDiscardingServerRpc()
    {
        GetHostPlayer().nrOfFinishedDiscards.Value++;
        OnFinishDiscardChanged?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ResetFinishedDiscardsServerRpc()
    {
        GetHostPlayer().nrOfFinishedDiscards.Value = 0;
        OnFinishDiscardChanged?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    public void HideDiscardWaitingCanvasServerRpc()
    {
        GetHostPlayer().HideDiscardWaitingCanvasClientRpc();
    }

    [ClientRpc]
    public void HideDiscardWaitingCanvasClientRpc()
    {
        var canvas = GameObject.Find("DiscardWaitingCanvas");
        if (canvas != null)
        {
            canvas.SetActive(false);
        }
    }

    private void InitializeDevelopmentDeck()
    {
        developmentsDeck = new List<string>
        {
            "Knight", "Knight", "Knight", "Knight", "Knight",
            "Knight", "Knight", "Knight", "Knight", "Knight",
            "Knight", "Knight", "Knight", "Knight", "Knight",
            "Monopoly", "Monopoly", "Monopoly",
            "RoadBuilding", "RoadBuilding", "RoadBuilding",
            "YearOfPlenty", "YearOfPlenty", "YearOfPlenty",
            "Chapel",
            "GreatHall",
            "Library",
            "Market",
            "University"
        };
    }

    [ServerRpc(RequireOwnership = false)]
    public void GetDevelopmentServerRpc(ServerRpcParams srp)
    {
        if (developmentsDeck.Count == 0)
        {
            print("no more developments left");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, developmentsDeck.Count);
        string development = developmentsDeck[randomIndex];
        developmentsDeck.RemoveAt(randomIndex);

        GetHostPlayer().GetDevelopmentClientRpc(
            development,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { srp.Receive.SenderClientId } }
            });
    }

    [ClientRpc]
    public void GetDevelopmentClientRpc(string development, ClientRpcParams crp)
    {
        //find the deck (it can be disabled if the tab is not active)
        var deckGroup = Resources.FindObjectsOfTypeAll<DeckGroup>()[0];

        var deckName = DevelopmentToDeck(development);

        GameObject deck = null;

        foreach (Transform child in deckGroup.transform)
        {
            if (child.gameObject.name == deckName)
            {
                deck = child.gameObject;
                break;
            }
        }

        if (deck == null)
        {
            print("No such deck");
            return;
        }

        //add the card
        var developmentGO = GameObject.Instantiate(DevelopmentToPrefab(development));

        developmentGO.transform.SetParent(deck.transform);

        //resize the deck
        var deckRectTransform = deck.transform as RectTransform;

        float width = 0;

        if (deckRectTransform.sizeDelta.x < 0)
        {
            width = 120;
        }
        else if (deckRectTransform.sizeDelta.x >= 100)
        {
            width = 25;
        }

        deckRectTransform.sizeDelta = new Vector2(deckRectTransform.sizeDelta.x + width, deckRectTransform.sizeDelta.y);
    }

    private string DevelopmentToDeck(string development)
    {
        switch (development)
        {
            case "Knight": return "KnightDeck";
            case "Monopoly": return "MonopolyDeck";
            case "RoadBuilding": return "RoadBuildingDeck";
            case "YearOfPlenty": return "YearOfPlentyDeck";
            case "Chapel": return "VictoryPointDeck";
            case "GreatHall": return "VictoryPointDeck";
            case "Library": return "VictoryPointDeck";
            case "Market": return "VictoryPointDeck";
            case "University": return "VictoryPointDeck";
            default: return null;
        }
    }

    private GameObject DevelopmentToPrefab(string development)
    {
        switch (development)
        {
            case "Knight": return knightPrefab;
            case "Monopoly": return monopolyPrefab;
            case "RoadBuilding": return roadBuildingPrefab;
            case "YearOfPlenty": return yearOfPlentyPrefab;
            case "Chapel": return chapelPrefab;
            case "GreatHall": return greatHallPrefab;
            case "Library": return libraryPrefab;
            case "Market": return marketPrefab;
            case "University": return universityPrefab;
            default: return null;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void MonopolyServerRpc(string resource, ServerRpcParams srp)
    {
        List<ulong> ids = new List<ulong>();

        for (ulong i = 0; i < (ulong)GetHostPlayer().nrOfMaxPlayers; i++)
        {
            ids.Add(i);
        }

        ids.Remove(srp.Receive.SenderClientId);

        GetHostPlayer().MonopolyClientRpc(
            resource,
            srp.Receive.SenderClientId,
            new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = ids }
            });
    }

    [ClientRpc]
    public void MonopolyClientRpc(string resource, ulong senderId, ClientRpcParams crp)
    {
        var myPlayer = GetMyPlayer();

        int nrOfCards = myPlayer.playerHand[resource];

        myPlayer.playerHand[resource] = 0;

        myPlayer.UpdateHand();

        var messageBoard = Resources.FindObjectsOfTypeAll<MonopolyMessageManager>()[0];

        messageBoard.SetMessage(GetPlayerWithId(senderId).nickName.Value.ToString(), resource);

        messageBoard.gameObject.SetActive(true);

        GetHostPlayer().AddResourcesToPlayerServerRpc(resource, nrOfCards, senderId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddResourcesToPlayerServerRpc(string resource, int nrOfCards, ulong targetId)
    {
        GetHostPlayer().AddResourcesToPlayerClientRpc(resource, nrOfCards, new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { targetId } }
        });
    }

    [ClientRpc]
    public void AddResourcesToPlayerClientRpc(string resource, int nrOfCards, ClientRpcParams crp)
    {
        var player = GetMyPlayer();

        player.playerHand[resource] += nrOfCards;
        player.UpdateHand();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DisplayDiceServerRpc(int redDice, int yellowDice)
    {
        GetHostPlayer().DisplayDiceClientRpc(redDice, yellowDice);
    }

    [ClientRpc]
    public void DisplayDiceClientRpc(int redDice, int yellowDice)
    {
        GameObject.Find("RedDice").GetComponent<UnityEngine.UI.Image>().sprite = NumberToRedDiceSprite(redDice);
        GameObject.Find("YellowDice").GetComponent<UnityEngine.UI.Image>().sprite = NumberToYellowDiceSprite(yellowDice);
    }

    private Sprite NumberToRedDiceSprite(int number)
    {
        switch (number)
        {
            case 1: return redDice1;
            case 2: return redDice2;
            case 3: return redDice3;
            case 4: return redDice4;
            case 5: return redDice5;
            case 6: return redDice6;
            default: return null;
        }
    }

    private Sprite NumberToYellowDiceSprite(int number)
    {
        switch (number)
        {
            case 1: return yellowDice1;
            case 2: return yellowDice2;
            case 3: return yellowDice3;
            case 4: return yellowDice4;
            case 5: return yellowDice5;
            case 6: return yellowDice6;
            default: return null;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddVictoryPointServerRpc(ServerRpcParams srp)
    {
        GetPlayerWithId(srp.Receive.SenderClientId).nrOfVictoryPoints.Value++;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UsedKnightServerRpc(ServerRpcParams srp)
    {
        GetPlayerWithId(srp.Receive.SenderClientId).nrOfUsedKnights.Value++;

        List<(ulong id, int usedKnights)> players = new List<(ulong id, int usedKnights)>();

        var playersGOs = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in playersGOs)
        {
            players.Add((p.GetComponent<Player>().OwnerClientId, p.GetComponent<Player>().nrOfUsedKnights.Value));
        }

        var max = players.Max(p => p.usedKnights);

        if (max < 3)
        {
            return;
        }

        var apparitions = players.Where(p => p.usedKnights == max).Count();

        if (apparitions > 1)
        {
            return;
        }

        if (players.FirstOrDefault(p => p.usedKnights == max).id != srp.Receive.SenderClientId)
        {
            return;
        }

        Player oldPlayer = null;

        foreach (var p in playersGOs)
        {
            if (p.GetComponent<Player>().hasLargestArmy.Value)
            {
                oldPlayer = p.GetComponent<Player>();
            }
        }

        //remove card
        if (oldPlayer != null)
        {

            GetPlayerWithId(oldPlayer.OwnerClientId).hasLargestArmy.Value = false;
            GetPlayerWithId(oldPlayer.OwnerClientId).nrOfVictoryPoints.Value -= 2;

            GetHostPlayer().RemovePointCardClientRpc("Largest Army", new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { oldPlayer.OwnerClientId } }
            });
        }


        //add card
        GetPlayerWithId(srp.Receive.SenderClientId).hasLargestArmy.Value = true;
        GetPlayerWithId(srp.Receive.SenderClientId).nrOfVictoryPoints.Value += 2;


        GetHostPlayer().AddPointCardClientRpc("Largest Army", new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { srp.Receive.SenderClientId } }
        });
    }

    [ClientRpc]
    public void RemovePointCardClientRpc(string type, ClientRpcParams crp)
    {
        if (type == "Largest Army")
        {
            GameObject.Find("LargestArmy").gameObject.SetActive(false);
        }
        else if (type == "Longest Road")
        {
            GameObject.Find("LongestRoad").gameObject.SetActive(false);

        }
    }

    [ClientRpc]
    public void AddPointCardClientRpc(string type, ClientRpcParams crp)
    {
        if (type == "Largest Army")
        {
            Resources.FindObjectsOfTypeAll<LargestArmy>()[0].gameObject.SetActive(true);
        }
        else if (type == "Longest Road")
        {
            Resources.FindObjectsOfTypeAll<LongestRoad>()[0].gameObject.SetActive(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void VictoryServerRpc(string winnerName)
    {
        GetHostPlayer().VictoryClientRpc(winnerName);
    }

    [ClientRpc]
    public void VictoryClientRpc(string winnerName)
    {
        var victoryBoard = Resources.FindObjectsOfTypeAll<VictoryManager>()[0];
        victoryBoard.GetComponent<VictoryManager>().SetMessage(winnerName);
        victoryBoard.gameObject.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendChatMessageServerRpc(string msg, ServerRpcParams srp)
    {
        var color = IdToHexColor(srp.Receive.SenderClientId);
        var name = GetPlayerWithId(srp.Receive.SenderClientId).nickName.Value.ToString();

        msg = "<color=" + color + ">" + name + ": </color>" + msg;

        GetHostPlayer().SendChatMessageClientRpc(msg);
    }

    [ClientRpc]
    public void SendChatMessageClientRpc(string msg)
    {
        var messageGO = Instantiate(messagePrefab);
        messageGO.GetComponent<TextMeshProUGUI>().text = msg;
        messageGO.transform.SetParent(Resources.FindObjectsOfTypeAll<ChatContent>()[0].transform);

        if (!GameObject.Find("Chat"))
        {
            Resources.FindObjectsOfTypeAll<NewMessagesDot>()[0].gameObject.SetActive(true);
        }
    }

    #region Longest Road
    private void CalculateLongestRoad()
    {
        var roadGrid = this.roadGrid.GetComponent<RoadGrid>().roadGrid;

        int longestRoad = 0;

        foreach (var row in roadGrid)
        {
            foreach (var road in row)
            {
                if (road == null)
                {
                    continue;
                }

                var roadDetails = road.GetComponent<RoadDetails>();

                if (roadDetails == null)
                {
                    continue;
                }

                if (roadDetails.color != GetMyPlayer().color)
                {
                    continue;
                }

                if (roadDetails.isVisited)
                {
                    continue;
                }

                int roadLength = ConexComponentLongestRoad(road);

                longestRoad = roadLength > longestRoad ? roadLength : longestRoad;
            }
        }

        TurnAllRoadsUnvisited(roadGrid);

        print("Longest road is: " +  longestRoad);
        GetHostPlayer().LongestRoadServerRpc(longestRoad, new ServerRpcParams());
    }

    private int ConexComponentLongestRoad(GameObject road)
    {
        Queue<GameObject> roadsQueue = new Queue<GameObject>();

        roadsQueue.Enqueue(road);

        Dictionary<string, List<string>> adjancencyList = new Dictionary<string, List<string>>();

        while (roadsQueue.Count != 0)
        {
            GameObject currentRoad = roadsQueue.Dequeue();

            currentRoad.GetComponent<RoadDetails>().isVisited = true;

            var nearbyBuildings = Physics.OverlapSphere(
                currentRoad.transform.position,
                2,
                (int)
                (Mathf.Pow(2, LayerMask.NameToLayer("My Settlement")) +
                Mathf.Pow(2, LayerMask.NameToLayer("City")) +
                Mathf.Pow(2, LayerMask.NameToLayer("Unvisible Circle")) +
                Mathf.Pow(2, LayerMask.NameToLayer("Settlement Circle")))
            );

            var nearbyRoads = Physics.OverlapSphere(
                currentRoad.transform.position,
                2,
                (int)
                Mathf.Pow(2, LayerMask.NameToLayer("Road"))
            );

            if (Has2NodesNearby(nearbyBuildings, currentRoad.GetComponent<RoadDetails>().color)) //contains only roads
            {
                //add to the current node's list
                if (!adjancencyList.ContainsKey(nearbyBuildings[0].name))
                {
                    adjancencyList[nearbyBuildings[0].name] = new List<string> { nearbyBuildings[1].name };
                }
                else
                {
                    if (!adjancencyList[nearbyBuildings[0].name].Contains(nearbyBuildings[1].name))
                    {
                        adjancencyList[nearbyBuildings[0].name].Add(nearbyBuildings[1].name);
                    }
                }

                //add to the other node as well
                if (!adjancencyList.ContainsKey(nearbyBuildings[1].name))
                {
                    adjancencyList[nearbyBuildings[1].name] = new List<string> { nearbyBuildings[0].name };
                }
                else
                {
                    if (!adjancencyList[nearbyBuildings[1].name].Contains(nearbyBuildings[0].name))
                    {
                        adjancencyList[nearbyBuildings[1].name].Add(nearbyBuildings[0].name);
                    }
                }

                //iterate over the next nearby roads
                foreach (var nearbyRoad in nearbyRoads)
                {
                    var roadDetails = nearbyRoad.GetComponent<RoadDetails>();

                    if (roadDetails.color != currentRoad.GetComponent<RoadDetails>().color)
                    {
                        continue;
                    }

                    if (roadDetails.isVisited)
                    {
                        continue;
                    }

                    roadsQueue.Enqueue(nearbyRoad.gameObject);
                }
            }
            else if (IsInterrupted(currentRoad))
            {
                //print("I've been interrupted");

                var interruptedNodes = Physics.OverlapSphere(
                    currentRoad.transform.position,
                    2,
                    (int)
                    (Mathf.Pow(2, LayerMask.NameToLayer("City")) +
                    Mathf.Pow(2, LayerMask.NameToLayer("Settlement")) +
                    Mathf.Pow(2, LayerMask.NameToLayer("Unvisible Circle")) +
                    Mathf.Pow(2, LayerMask.NameToLayer("Settlement Circle")))
                );

                //print("We should be " + interruptedNodes[0].name + " " + interruptedNodes[1].name);

                //add to the current node's list
                if (!adjancencyList.ContainsKey(interruptedNodes[0].name))
                {
                    adjancencyList[interruptedNodes[0].name] = new List<string> { interruptedNodes[1].name };
                }
                else
                {
                    if (!adjancencyList[interruptedNodes[0].name].Contains(interruptedNodes[1].name))
                    {
                        adjancencyList[interruptedNodes[0].name].Add(interruptedNodes[1].name);
                    }
                }

                //add to the other node as well
                if (!adjancencyList.ContainsKey(interruptedNodes[1].name))
                {
                    adjancencyList[interruptedNodes[1].name] = new List<string> { interruptedNodes[0].name };
                }
                else
                {
                    if (!adjancencyList[interruptedNodes[1].name].Contains(interruptedNodes[0].name))
                    {
                        adjancencyList[interruptedNodes[1].name].Add(interruptedNodes[0].name);
                    }
                }
            }

        }

        //foreach (var key in adjancencyList.Keys)
        //{
        //    print("Cheie: " + key);

        //    foreach (var neighbour in adjancencyList[key])
        //    {
        //        print(neighbour);
        //    }
        //}

        TurnRoadsVisited(adjancencyList, false);

        int longestRoad = LongestPath(adjancencyList);

        //print("Cel mai lung drum este: " + (longestRoad - 1));

        return longestRoad - 1;
    }

    private bool Has2NodesNearby(Collider[] buildings, Color color)
    {
        int counter = 0;

        foreach (var building in buildings)
        {
            var cityPiece = building.GetComponent<CityPiece>();
            if (cityPiece != null)
            {
                if (cityPiece.color != color)
                {
                    continue;
                }
            }

            counter++;
        }

        return counter == 2;
    }

    private bool IsInterrupted(GameObject road)
    {
        int counter = 0;
        bool isEnemyBuilding = false;

        var nearbyBuildings = Physics.OverlapSphere(
                road.transform.position,
                2,
                (int)
                (Mathf.Pow(2, LayerMask.NameToLayer("Settlement")) +
                Mathf.Pow(2, LayerMask.NameToLayer("City")) +
                Mathf.Pow(2, LayerMask.NameToLayer("Unvisible Circle")) +
                Mathf.Pow(2, LayerMask.NameToLayer("Settlement Circle")))
        );

        foreach (var building in nearbyBuildings)
        {
            var cityPiece = building.GetComponent<CityPiece>();
            if (cityPiece != null)
            {
                if (cityPiece.color != color)
                {
                    isEnemyBuilding = true;
                    continue;
                }
            }

            //there should be only enemy settlements
            var settlementPiece = building.GetComponent<SettlementPiece>();
            if (settlementPiece != null)
            {
                isEnemyBuilding = true;
                continue;
            }

            counter++;
        }

        return isEnemyBuilding && (counter == 1);
    }

    private void TurnAllRoadsUnvisited(GameObject[][] roadGrid)
    {
        foreach (var row in roadGrid)
        {
            foreach (var road in row)
            {
                var roadDetails = road.GetComponent<RoadDetails>();

                if (roadDetails == null)
                {
                    continue;
                }

                roadDetails.isVisited = false;
            }
        }
    }

    private int MaxPathDFS(Dictionary<string, List<string>> adjancencyList, string node)
    {

        var dfsDetails = GameObject.Find(node).GetComponent<DFSDetails>();

        if (dfsDetails == null)
        {
            dfsDetails = settlementGrid.transform.Find(node).GetComponent<DFSDetails>();
        }

        if (dfsDetails.isVisited)
        {
            return 0;
        }

        dfsDetails.isVisited = true;

        int maxPath = 0;

        foreach (var neighbour in adjancencyList[node])
        {
            int dfsCallback = MaxPathDFS(adjancencyList, neighbour) + 1;
            maxPath = dfsCallback > maxPath ? dfsCallback : maxPath;
        }

        return maxPath;
    }

    private void TurnRoadsVisited(Dictionary<string, List<string>> adjancencyList, bool visited)
    {
        foreach (var node in adjancencyList.Keys)
        {
            var dfsDetails = GameObject.Find(node).GetComponent<DFSDetails>();

            if (dfsDetails == null)
            {
                dfsDetails = settlementGrid.transform.Find(node).GetComponent<DFSDetails>();
            }

            dfsDetails.isVisited = visited;
        }
    }

    private int LongestPath(Dictionary<string, List<string>> adjancencyList)
    {
        int longestPath = 0;

        foreach (var node1 in adjancencyList.Keys)
        {
            foreach (var node2 in adjancencyList.Keys)
            {
                var dfsDetails = GameObject.Find(node2).GetComponent<DFSDetails>();

                if (dfsDetails == null)
                {
                    settlementGrid.transform.Find(node2).GetComponent<DFSDetails>().isVisited = false;
                }
                else
                {
                    dfsDetails.isVisited = false;
                }
            }

            int dfsLength = MaxPathDFS(adjancencyList, node1);

            longestPath = dfsLength > longestPath ? dfsLength : longestPath;
        }

        return longestPath;
    }
    #endregion

    [ServerRpc(RequireOwnership = false)]
    public void LongestRoadServerRpc(int longestRoad, ServerRpcParams srp)
    {
        GetPlayerWithId(srp.Receive.SenderClientId).longestRoad.Value = longestRoad;

        List<(ulong id, int longestRoad)> players = new List<(ulong id, int longestRoad)>();

        var playersGOs = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in playersGOs)
        {
            players.Add((p.GetComponent<Player>().OwnerClientId, p.GetComponent<Player>().longestRoad.Value));
        }

        var max = players.Max(p => p.longestRoad);

        if (max < 5)
        {
            return;
        }

        var apparitions = players.Where(p => p.longestRoad == max).Count();

        if (apparitions > 1)
        {
            return;
        }

        if (players.FirstOrDefault(p => p.longestRoad == max).id != srp.Receive.SenderClientId)
        {
            return;
        }

        Player oldPlayer = null;

        foreach (var p in playersGOs)
        {
            if (p.GetComponent<Player>().hasLongestRoad.Value)
            {
                oldPlayer = p.GetComponent<Player>();
            }
        }

        //remove card
        if (oldPlayer != null)
        {

            GetPlayerWithId(oldPlayer.OwnerClientId).hasLongestRoad.Value = false;
            GetPlayerWithId(oldPlayer.OwnerClientId).nrOfVictoryPoints.Value -= 2;

            GetHostPlayer().RemovePointCardClientRpc("Longest Road", new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { oldPlayer.OwnerClientId } }
            });
        }


        //add card
        GetPlayerWithId(srp.Receive.SenderClientId).hasLongestRoad.Value = true;
        GetPlayerWithId(srp.Receive.SenderClientId).nrOfVictoryPoints.Value += 2;


        GetHostPlayer().AddPointCardClientRpc("Longest Road", new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { srp.Receive.SenderClientId } }
        });
    }
}
