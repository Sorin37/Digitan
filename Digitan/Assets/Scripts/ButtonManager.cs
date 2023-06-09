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

    [SerializeField] private Canvas tradeCanvas;
    [SerializeField] private GameObject tradeManager;
    [SerializeField] private GameObject chat;
    [SerializeField] private GameObject tips;

    [SerializeField] private GameObject notEnoughResourcesPrefab;
    [SerializeField] private GameObject notYourTurnPrefab;
    [SerializeField] private GameObject rollTheDicePrefab;
    [SerializeField] private GameObject alreadyRolledPrefab;
    private bool hasRolledDice = false;

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

            Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Settlement Circle"));

        });
    }
    private void InitRoadButton()
    {
        roadButton.onClick.AddListener(() =>
        {
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
        var playerHand = GetMyPlayer().GetComponent<Player>().playerHand;
        if (playerHand["Brick Resource"] > 0 &&
            playerHand["Grain Resource"] > 0 &&
            playerHand["Lumber Resource"] > 0 &&
            playerHand["Wool Resource"] > 0)
        {
            playerHand["Brick Resource"]--;
            playerHand["Grain Resource"]--;
            playerHand["Lumber Resource"]--;
            playerHand["Wool Resource"]--;

            GetMyPlayer().GetComponent<Player>().UpdateHand();

            return true;
        }

        return false;
    }

    private bool HasRoadResources()
    {
        var playerHand = GetMyPlayer().GetComponent<Player>().playerHand;

        if (playerHand["Brick Resource"] > 0 && playerHand["Lumber Resource"] > 0)
        {
            playerHand["Brick Resource"]--;
            playerHand["Lumber Resource"]--;

            GetMyPlayer().GetComponent<Player>().UpdateHand();

            return true;
        }

        return false;
    }

    private bool HasCityResources()
    {
        var playerHand = GetMyPlayer().GetComponent<Player>().playerHand;

        if (playerHand["Ore Resource"] > 2 && playerHand["Grain Resource"] > 1)
        {
            playerHand["Ore Resource"] -= 3;
            playerHand["Grain Resource"] -= 2;

            GetMyPlayer().GetComponent<Player>().UpdateHand();

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

    private bool IsMyTurn()
    {
        return GetHostPlayer().currentPlayerTurn.Value == (int)NetworkManager.Singleton.LocalClientId;
    }

    private bool HasDevelopmentResources()
    {
        var playerHand = GetMyPlayer().GetComponent<Player>().playerHand;

        if (playerHand["Grain Resource"] > 0 &&
            playerHand["Ore Resource"] > 0 &&
            playerHand["Wool Resource"] > 0)
        {
            playerHand["Grain Resource"]--;
            playerHand["Ore Resource"]--;
            playerHand["Wool Resource"]--;

            GetMyPlayer().GetComponent<Player>().UpdateHand();

            return true;
        }

        return false;
    }
}
