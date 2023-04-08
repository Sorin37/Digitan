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
            if (GetHostPlayer().GetComponent<Player>().currentPlayerTurn.Value != (int)NetworkManager.Singleton.LocalClientId)
            {
                print("nu e runda mea :/");
                return;
            }
            print("a fost runda mea hehe");
            GetHostPlayer().GetComponent<Player>().PassTurnServerRpc();
        });
    }
    private void initSettlementButton()
    {
        settlementButton.onClick.AddListener(() =>
        {
            if (GetHostPlayer().GetComponent<Player>().currentPlayerTurn.Value != (int)NetworkManager.Singleton.LocalClientId)
            {
                print("nu e runda mea :/");
                return;
            }

            if (hasSettlementResources())
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
            if (GetHostPlayer().GetComponent<Player>().currentPlayerTurn.Value != (int)NetworkManager.Singleton.LocalClientId)
            {
                print("nu e runda mea :/");
                return;
            }

            if (hasRoadResources())
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
            tradeCanvas.gameObject.SetActive(true);
        });
    }
    private void initCityButton()
    {
        cityButton.onClick.AddListener(() =>
        {
            print("se fac orase");
        });
    }

    bool hasSettlementResources()
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

    bool hasRoadResources()
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
}
