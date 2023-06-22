using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button settlementButton;
    [SerializeField] private Button roadButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button rollDiceButton;
    [SerializeField] private Button tradeButton;
    [SerializeField] private Button cityButton;
    [SerializeField] private Button developmentButton;
    [SerializeField] private Button chatButton;
    [SerializeField] private Button tipsButton;
    [SerializeField] private Button playersInfoButton;
    [SerializeField] private Button recipeButton;

    [SerializeField] private Canvas tradeCanvas;
    [SerializeField] private GameObject tradeManager;
    [SerializeField] private GameObject chat;
    [SerializeField] private GameObject tips;
    [SerializeField] private GameObject recipeCanvas;
    [SerializeField] private GameObject playersInfoCanvas;

    [SerializeField] private GameObject notEnoughResourcesPrefab;
    [SerializeField] private GameObject notYourTurnPrefab;
    [SerializeField] private GameObject rollTheDicePrefab;
    [SerializeField] private GameObject alreadyRolledPrefab;
    [SerializeField] private GameObject moveThiefPrefab;
    [SerializeField] private GameObject placeRoadPrefab;
    [SerializeField] private GameObject placeSettlementPrefab;
    [SerializeField] private GameObject placeCityPrefab;
    [SerializeField] private GameObject noMorePiecesPrefab;

    [SerializeField] private GameObject playerInfoPrefab;

    public bool hasRolledDice = false;

    private void Awake()
    {
        InitEndTurnButton();
        InitRollDiceButton();
        InitSettlementButton();
        InitRoadButton();
        InitTradeButton();
        InitCityButton();
        InitDevelopmentButton();
        InitChatButton();
        InitTipsButton();
        InitRecipesButton();
        InitPlayerInfoButton();
    }

    private void InitRollDiceButton()
    {
        rollDiceButton.onClick.AddListener(() =>
        {
            if (!IsMyTurn())
            {
                var message = Instantiate(notYourTurnPrefab, rollDiceButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(rollDiceButton.transform);
                return;
            }

            if (hasRolledDice)
            {
                var message = Instantiate(alreadyRolledPrefab, rollDiceButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(rollDiceButton.transform);
                return;
            }

            hasRolledDice = true;

            GetHostPlayer().RollDiceServerRpc();
        });
    }

    private void InitEndTurnButton()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            if (!IsMyTurn())
            {
                var message = Instantiate(notYourTurnPrefab, endTurnButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(endTurnButton.transform);
                return;
            }

            if (!hasRolledDice)
            {
                var message = Instantiate(rollTheDicePrefab, endTurnButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(endTurnButton.transform);
                return;
            }

            if (GetMyPlayer().hasToMoveThief)
            {
                var message = Instantiate(moveThiefPrefab, endTurnButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(endTurnButton.transform);
                return;
            }

            if (GetMyPlayer().hasToPlaceRoad)
            {
                var message = Instantiate(placeRoadPrefab, endTurnButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(endTurnButton.transform);
                return;
            }

            if (GetMyPlayer().hasToPlaceSettlement)
            {
                var message = Instantiate(placeSettlementPrefab, endTurnButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(endTurnButton.transform);
                return;
            }

            if (GetMyPlayer().hasToPlaceCity)
            {
                var message = Instantiate(placeCityPrefab, endTurnButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(endTurnButton.transform);
                return;
            }

            hasRolledDice = false;

            GetHostPlayer().PassTurnServerRpc();
        });
    }
    private void InitSettlementButton()
    {
        settlementButton.onClick.AddListener(() =>
        {
            if (!IsMyTurn())
            {
                var message = Instantiate(notYourTurnPrefab, settlementButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(settlementButton.transform);
                return;
            }

            if (!hasRolledDice)
            {
                var message = Instantiate(rollTheDicePrefab, settlementButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(settlementButton.transform);
                return;
            }

            if (!HasSettlementResources())
            {
                var message = Instantiate(notEnoughResourcesPrefab, settlementButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(settlementButton.transform);
                return;
            }

            if (GetMyPlayer().nrOfPlacedCities == 4)
            {
                var message = Instantiate(noMorePiecesPrefab, settlementButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(settlementButton.transform);
                return;
            }

            Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Settlement Circle"));
            GetMyPlayer().hasToPlaceSettlement = true;
        });
    }
    private void InitRoadButton()
    {
        roadButton.onClick.AddListener(() =>
        {
            var myPlayer = GetMyPlayer();

            if (!IsMyTurn())
            {
                var message = Instantiate(notYourTurnPrefab, roadButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(roadButton.transform);
                return;
            }

            if (!hasRolledDice)
            {
                var message = Instantiate(rollTheDicePrefab, roadButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(roadButton.transform);
                return;
            }

            if (!HasRoadResources())
            {
                var message = Instantiate(notEnoughResourcesPrefab, roadButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(roadButton.transform);
                return;
            }

            if (myPlayer.nrOfPlacedRoads == 15)
            {
                var message = Instantiate(noMorePiecesPrefab, roadButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(roadButton.transform);
                return;
            }

            if (myPlayer.hasToPlaceRoad)
            {
                Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Road Circle"));
             
                myPlayer.playerHand["Brick Resource"]++;
                myPlayer.playerHand["Lumber Resource"]++;

                myPlayer.UpdateHand();

                myPlayer.hasToPlaceRoad = false;

                return;
            }

            myPlayer.hasToPlaceRoad = true;

            Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Road Circle"));
        });
    }
    private void InitTradeButton()
    {
        tradeButton.onClick.AddListener(() =>
        {
            if (!IsMyTurn())
            {
                var message = Instantiate(notYourTurnPrefab, tradeButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(tradeButton.transform);
                return;
            }

            if (!hasRolledDice)
            {
                var message = Instantiate(rollTheDicePrefab, tradeButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(tradeButton.transform);
                return;
            }

            ResetTrade();
            tradeCanvas.gameObject.SetActive(true);
        });
    }
    private void InitCityButton()
    {
        cityButton.onClick.AddListener(() =>
        {
            if (!IsMyTurn())
            {
                var message = Instantiate(notYourTurnPrefab, cityButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(cityButton.transform);
                return;
            }

            if (!hasRolledDice)
            {
                var message = Instantiate(rollTheDicePrefab, cityButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(cityButton.transform);
                return;
            }

            if (!HasCityResources())
            {
                var message = Instantiate(notEnoughResourcesPrefab, cityButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(cityButton.transform);
                return;
            }

            Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("City Place"));
            Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("My Settlement"));
            GetMyPlayer().hasToPlaceCity = true;
        });
    }

    private void InitDevelopmentButton()
    {
        developmentButton.onClick.AddListener(() =>
        {
            if (!IsMyTurn())
            {
                var message = Instantiate(notYourTurnPrefab, developmentButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(developmentButton.transform);
                return;
            }

            if (!HasDevelopmentResources())
            {
                var message = Instantiate(notEnoughResourcesPrefab, developmentButton.transform);
                message.GetComponent<RedMessage>().SetStartPosition(developmentButton.transform);
                return;
            }

            GetHostPlayer().GetDevelopmentServerRpc(new ServerRpcParams());
        });
    }

    private void InitChatButton()
    {
        chatButton.onClick.AddListener(() =>
        {
            chat.gameObject.SetActive(!chat.gameObject.activeSelf);
            GameObject.Find("NewMessagesDot")?.gameObject.SetActive(false);
        });
    }

    private void InitTipsButton()
    {
        tipsButton.onClick.AddListener(() =>
        {
            tips.GetComponent<TipsManager>().GenerateTip();
            tips.gameObject.SetActive(!tips.gameObject.activeSelf);
        });
    }

    private bool HasSettlementResources()
    {
        var playerHand = GetMyPlayer().playerHand;
        if (playerHand["Brick Resource"] > 0 &&
            playerHand["Grain Resource"] > 0 &&
            playerHand["Lumber Resource"] > 0 &&
            playerHand["Wool Resource"] > 0)
        {
            playerHand["Brick Resource"]--;
            playerHand["Grain Resource"]--;
            playerHand["Lumber Resource"]--;
            playerHand["Wool Resource"]--;

            GetMyPlayer().UpdateHand();

            return true;
        }

        return false;
    }

    private bool HasRoadResources()
    {
        var playerHand = GetMyPlayer().playerHand;

        if (playerHand["Brick Resource"] > 0 && playerHand["Lumber Resource"] > 0)
        {
            playerHand["Brick Resource"]--;
            playerHand["Lumber Resource"]--;

            GetMyPlayer().UpdateHand();

            return true;
        }

        return false;
    }

    private bool HasCityResources()
    {
        var playerHand = GetMyPlayer().playerHand;

        if (playerHand["Ore Resource"] > 2 && playerHand["Grain Resource"] > 1)
        {
            playerHand["Ore Resource"] -= 3;
            playerHand["Grain Resource"] -= 2;

            GetMyPlayer().UpdateHand();

            return true;
        }

        return false;
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

    private void ResetTrade()
    {
        var tradeManager = this.tradeManager.GetComponent<TradeManager>();
        var giveDict = tradeManager.giveDict;
        var getDict = tradeManager.getDict;

        giveDict["Brick"] = 0;
        giveDict["Grain"] = 0;
        giveDict["Lumber"] = 0;
        giveDict["Ore"] = 0;
        giveDict["Wool"] = 0;

        getDict["Brick"] = 0;
        getDict["Grain"] = 0;
        getDict["Lumber"] = 0;
        getDict["Ore"] = 0;
        getDict["Wool"] = 0;

        GetHostPlayer().nrOfDeclinedTrades = 0;

        tradeManager.DrawDicts();
    }

    public bool IsMyTurn()
    {
        return GetHostPlayer().currentPlayerTurn.Value == (int)NetworkManager.Singleton.LocalClientId;
    }

    private bool HasDevelopmentResources()
    {
        var playerHand = GetMyPlayer().playerHand;

        if (playerHand["Grain Resource"] > 0 &&
            playerHand["Ore Resource"] > 0 &&
            playerHand["Wool Resource"] > 0)
        {
            playerHand["Grain Resource"]--;
            playerHand["Ore Resource"]--;
            playerHand["Wool Resource"]--;

            GetMyPlayer().UpdateHand();

            return true;
        }

        return false;
    }

    private void InitRecipesButton()
    {
        recipeButton.onClick.AddListener(() =>
        {
            recipeCanvas.SetActive(true);
        });
    }

    private void InitPlayerInfoButton()
    {
        playersInfoButton.onClick.AddListener(() =>
        {
            //unity bug unfortunately, that's a work around
            playersInfoCanvas.SetActive(true);
            playersInfoCanvas.SetActive(false);

            DisplayPlayersInfo();

            playersInfoCanvas.SetActive(true);

        });
    }

    private void DisplayPlayersInfo()
    {
        var nrOfmaxPlayers = (ulong)GetMyPlayer().nrOfMaxPlayers;

        var playerInfoBoard = playersInfoCanvas.transform.Find("Board");

        //to get all children
        foreach (Transform child in playerInfoBoard.transform)
        {
            if (child.gameObject.name == "Player Info")
            {
                Destroy(child.gameObject);
            }
        }

        for (ulong i = 0; i < nrOfmaxPlayers; i++)
        {
            var playerInfoGO = Instantiate(playerInfoPrefab);

            playerInfoGO.GetComponent<PlayerInfo>().SetInfo(i);

            playerInfoGO.name = "Player Info";

            playerInfoGO.transform.SetParent(playerInfoBoard);
        }
    }
}
