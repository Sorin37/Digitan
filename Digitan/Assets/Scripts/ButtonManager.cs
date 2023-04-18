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
    [SerializeField] private Button tradeButton;
    [SerializeField] private Button cityButton;
    [SerializeField] private Canvas tradeCanvas;
    [SerializeField] private GameObject tradeManager;

    private void Awake()
    {
        initEndTurnButton();
        initSettlementButton();
        initRoadButton();
        initTradeButton();
        initCityButton();
    }

    private void initEndTurnButton()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            if (!IsMyTurn())
            {
                return;
            }
            GetHostPlayer().GetComponent<Player>().PassTurnServerRpc();
        });
    }
    private void initSettlementButton()
    {
        settlementButton.onClick.AddListener(() =>
        {
            if (!IsMyTurn())
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
    private void initRoadButton()
    {
        roadButton.onClick.AddListener(() =>
        {
            if (!IsMyTurn())
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
    private void initTradeButton()
    {
        tradeButton.onClick.AddListener(() =>
        {
            if (!IsMyTurn())
            {
                return;
            }
            ResetTrade();
            tradeCanvas.gameObject.SetActive(true);
        });
    }
    private void initCityButton()
    {
        cityButton.onClick.AddListener(() =>
        {
            print("se fac orase");
            //if (HasCityResources())
            //{
            Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("City Place"));
            //}
            //else
            //{
            //    print("You don't have enough resources for a road!");
            //}
        });
    }

    bool HasSettlementResources()
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

    bool HasRoadResources()
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

        GetHostPlayer().GetComponent<Player>().nrOfDeclinedTrades = 0;

        tradeManager.DrawDicts();
    }

    private bool IsMyTurn()
    {
        return GetHostPlayer().GetComponent<Player>().currentPlayerTurn.Value == (int)NetworkManager.Singleton.LocalClientId;
    }
}
