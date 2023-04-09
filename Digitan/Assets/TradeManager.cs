using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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


    public Dictionary<string, int> giveDict = new Dictionary<string, int>();
    public Dictionary<string, int> getDict = new Dictionary<string, int>();

    // Start is called before the first frame update
    void Start()
    {
        InitCloseButton();
        InitTradeBankButton();
        InitTradePlayersButton();
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
            print("I be tradign with the palyers");
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
}
