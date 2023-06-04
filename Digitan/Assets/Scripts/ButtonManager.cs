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
            if (!IsMyTurn() || hasRolledDice)
            {
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
            if (!IsMyTurn() || !hasRolledDice)
                return;

            hasRolledDice = false;

            GetHostPlayer().PassTurnServerRpc();
        });
    }
    private void InitSettlementButton()
    {
        settlementButton.onClick.AddListener(() =>
        {
            if (!IsMyTurn() || !hasRolledDice)
            {
                return;
            }

            if (HasSettlementResources())
            {
                Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Settlement Circle"));
            }
            else
            {
                print("You don't have enough resources for a settlement!");
            }
        });
    }
    private void InitRoadButton()
    {
        roadButton.onClick.AddListener(() =>
        {
            if (!IsMyTurn() || !hasRolledDice)
            {
                return;
            }

            if (HasRoadResources())
            {
                Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Road Circle"));
            }
            else
            {
                print("You don't have enough resources for a road!");
            }
        });
    }
    private void InitTradeButton()
    {
        tradeButton.onClick.AddListener(() =>
        {
            if (!IsMyTurn() || !hasRolledDice)
            {
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
            if (!IsMyTurn() || !hasRolledDice)
            {
                return;
            }

            if (HasCityResources())
            {
                Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("City Place"));
                Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("My Settlement"));
            }
            else
            {
                //todo: implement an error message or smth
                print("You don't have enough resources for a road!");
            }
        });
    }

    private void InitDevelopmentButton()
    {
        developmentButton.onClick.AddListener(() =>
        {
            if (!IsMyTurn())
            {
                return;
            }

            //if(!HasDevelopmentResources())
            //{
            //    return;
            //}

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
