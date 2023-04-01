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
using UnityEngine;
using UnityEngine.UI;

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
    public NetworkVariable<int> currentNrOfPlayers = new NetworkVariable<int>(0);
    public NetworkVariable<int> order = new NetworkVariable<int>(1);
    public NetworkVariable<int> currentPlayerTurn = new NetworkVariable<int>(-1);

    public Dictionary<string, List<string>> resourcesDict;
    public Dictionary<string, int> playerHand;

    public int nrOfMaxPlayers;
    public string nickName;
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

        if (IsOwner)
        {
            OnRoundEnd += EndTurnEvent;
            
        }

        if (!IsOwnedByServer)
            return;

        //this is where you should do normal initialisations
        DontDestroyOnLoad(gameObject);

        if (IsHost)
        {
            resourcesCode.Value = GenerateResourcesCode();
        }

        gameGrid.GetComponent<GameGrid>().CreateGrid(resourcesCode.Value.ToString());

        await PopInformationFromLobby();

        print(nickName);

        PlayerJoinedServerRpc();
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
    public void placeSettlementServerRpc(int x, int y, Color color)
    {
        placeSettlementClientRpc(x, y, color);
    }

    [ClientRpc]
    public void placeSettlementClientRpc(int x, int y, Color color)
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
                nickName = player.Data["PlayerName"].Value;
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
        ChangeCurrentPlayerDetailsNameClientRpc(GetHostPlayer().GetComponent<Player>().nickName);
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
    public void PlacedServerRpc(string name)
    {
        var player = GetHostPlayer().GetComponent<Player>();

        if ((int)OwnerClientId + player.order.Value == player.nrOfMaxPlayers)
        {
            player.order.Value = -1;
            ChangeCurrentPlayerDetailsColorClientRpc(OwnerClientId);
            ChangeCurrentPlayerDetailsNameClientRpc(name);
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
            ChangeCurrentPlayerDetailsNameClientRpc(name);
        }

        ChangeCurrentPlayerDetailsColorClientRpc((ulong)((int)OwnerClientId + player.order.Value));
        ChangeCurrentPlayerDetailsNameClientRpc(name);
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
}
