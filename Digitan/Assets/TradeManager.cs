using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TradeManager : MonoBehaviour
{
    [SerializeField] private Canvas tradeCanvas;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button tradeBankButton;
    [SerializeField] private Button tradePlayersButton;
    [SerializeField] private GameObject give;
    [SerializeField] private GameObject get;
    [SerializeField] private TextMeshProUGUI giveBrickLabel;
    [SerializeField] private TextMeshProUGUI giveGrainLabel;
    [SerializeField] private TextMeshProUGUI giveLumberLabel;
    [SerializeField] private TextMeshProUGUI giveOreLabel;
    [SerializeField] private TextMeshProUGUI giveWoolLabel;
    [SerializeField] private TextMeshProUGUI getBrickLabel;
    [SerializeField] private TextMeshProUGUI getGrainLabel;
    [SerializeField] private TextMeshProUGUI getLumberLabel;
    [SerializeField] private TextMeshProUGUI getOreLabel;
    [SerializeField] private TextMeshProUGUI getWoolLabel;
    [SerializeField] private Button subtractGiveBrick;
    [SerializeField] private Button addGiveBrick;
    [SerializeField] private Button subtractGiveGrain;
    [SerializeField] private Button addGiveGrain;
    [SerializeField] private Button subtractGiveLumber;
    [SerializeField] private Button addGiveLumber;
    [SerializeField] private Button subtractGiveOre;
    [SerializeField] private Button addGiveOre;
    [SerializeField] private Button subtractGiveWool;
    [SerializeField] private Button addGiveWool;
    [SerializeField] private Button subtractGetBrick;
    [SerializeField] private Button addGetBrick;
    [SerializeField] private Button subtractGetGrain;
    [SerializeField] private Button addGetGrain;
    [SerializeField] private Button subtractGetLumber;
    [SerializeField] private Button addGetLumber;
    [SerializeField] private Button subtractGetOre;
    [SerializeField] private Button addGetOre;
    [SerializeField] private Button subtractGetWool;
    [SerializeField] private Button addGetWool;

    public Dictionary<string, int> giveDict = new Dictionary<string, int>();
    public Dictionary<string, int> getDict = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Start()
    {
        InitCloseButton();
        InitTradeBankButton();
        InitTradePlayersButton();
        InitResourcesButtons();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitCloseButton()
    {
        closeButton.onClick.AddListener(() =>
        {
            tradeCanvas.gameObject.SetActive(false);
            GetHostPlayer().GetComponent<Player>().CancelTradeServerRpc();

        });
    }

    void InitTradeBankButton()
    {
        tradeBankButton.onClick.AddListener(() =>
        {
            print("I be tradign with the bank");
        });
    }

    void InitTradePlayersButton()
    {
        tradePlayersButton.onClick.AddListener(() =>
        {
            GetHostPlayer().GetComponent<Player>().DisplayTradeOfferServerRpc(
                NetworkManager.Singleton.LocalClientId,
                giveDict["Brick"], giveDict["Grain"], giveDict["Lumber"], giveDict["Ore"], giveDict["Wool"],
                getDict["Brick"], getDict["Grain"], getDict["Lumber"], getDict["Ore"], getDict["Wool"]
            );
        });
    }

    public void DrawDicts()
    {
        giveBrickLabel.text = "x " + giveDict["Brick"].ToString();
        giveGrainLabel.text = "x " + giveDict["Grain"].ToString();
        giveLumberLabel.text = "x " + giveDict["Lumber"].ToString();
        giveOreLabel.text = "x " + giveDict["Ore"].ToString();
        giveWoolLabel.text = "x " + giveDict["Wool"].ToString();

        getBrickLabel.text = "x " + getDict["Brick"].ToString();
        getGrainLabel.text = "x " + getDict["Grain"].ToString();
        getLumberLabel.text = "x " + getDict["Lumber"].ToString();
        getOreLabel.text = "x " + getDict["Ore"].ToString();
        getWoolLabel.text = "x " + getDict["Wool"].ToString();
    }

    private void Add(string type, string resource)
    {
        var playerHand = GetMyPlayer().GetComponent<Player>().playerHand;

        if (type == "Give")
        {
            if (playerHand[resource + " Resource"] > giveDict[resource])
            {
                giveDict[resource]++;
            }
        }
        else if (type == "Get")
        {
            if (playerHand[resource + " Resource"] > giveDict[resource])
            {
                giveDict[resource]++;
            }
        }
    }

    private void Subtract(string type, string resource)
    {
        if (type == "Give")
        {
            if (giveDict[resource] > 0)
                giveDict[resource]--;
        }
        else if (type == "Get")
        {
            if (getDict[resource] > 0)
                getDict[resource]--;
        }
    }

    private void InitResourcesButtons()
    {
        //give
        addGiveBrick.onClick.AddListener(() =>
        {
            Add("Give", "Brick");
            DrawDicts();
        });

        addGiveGrain.onClick.AddListener(() =>
        {
            Add("Give", "Grain");
            DrawDicts();
        });

        addGiveLumber.onClick.AddListener(() =>
        {
            Add("Give", "Lumber");
            DrawDicts();
        });

        addGiveOre.onClick.AddListener(() =>
        {
            Add("Give", "Ore");
            DrawDicts();
        });

        addGiveWool.onClick.AddListener(() =>
        {
            Add("Give", "Wool");
            DrawDicts();
        });

        //give subtract
        subtractGiveBrick.onClick.AddListener(() =>
        {
            Subtract("Give", "Brick");
            DrawDicts();
        });

        subtractGiveGrain.onClick.AddListener(() =>
        {
            Subtract("Give", "Grain");
            DrawDicts();
        });

        subtractGiveLumber.onClick.AddListener(() =>
        {
            Subtract("Give", "Lumber");
            DrawDicts();
        });

        subtractGiveOre.onClick.AddListener(() =>
        {
            Subtract("Give", "Ore");
            DrawDicts();
        });

        subtractGiveWool.onClick.AddListener(() =>
        {
            Subtract("Give", "Wool");
            DrawDicts();
        });

        //get add
        addGetBrick.onClick.AddListener(() =>
        {
            Add("Get", "Brick");
            DrawDicts();
        });

        addGetGrain.onClick.AddListener(() =>
        {
            Add("Get", "Grain");
            DrawDicts();
        });

        addGetLumber.onClick.AddListener(() =>
        {
            Add("Get", "Lumber");
            DrawDicts();
        });

        addGetOre.onClick.AddListener(() =>
        {
            Add("Get", "Ore");
            DrawDicts();
        });

        addGetWool.onClick.AddListener(() =>
        {
            Add("Get", "Wool");
            DrawDicts();
        });

        //get subtract
        subtractGetBrick.onClick.AddListener(() =>
        {
            Subtract("Get", "Brick");
            DrawDicts();
        });

        subtractGetGrain.onClick.AddListener(() =>
        {
            Subtract("Get", "Grain");
            DrawDicts();
        });

        subtractGetLumber.onClick.AddListener(() =>
        {
            Subtract("Get", "Lumber");
            DrawDicts();
        });

        subtractGetOre.onClick.AddListener(() =>
        {
            Subtract("Get", "Ore");
            DrawDicts();
        });

        subtractGetWool.onClick.AddListener(() =>
        {
            Subtract("Get", "Wool");
            DrawDicts();
        });
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
}
