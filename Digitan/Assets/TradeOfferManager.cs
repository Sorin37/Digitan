using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeOfferManager : MonoBehaviour
{
    [SerializeField] private Canvas tradeOfferCanvas;
    [SerializeField] private Button acceptTradeButton;
    [SerializeField] private Button declineTradeButton;
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

    public ulong tradeMakerId;

    // Start is called before the first frame update
    void Start()
    {
        InitDeclineTradeButton();
        InitAcceptTradeButton();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void InitAcceptTradeButton()
    {
        acceptTradeButton.onClick.AddListener(() =>
        {
            print("Da bro cu id: " + tradeMakerId + ", accept");
            tradeOfferCanvas.gameObject.SetActive(false);
        });
    }

    void InitDeclineTradeButton()
    {
        declineTradeButton.onClick.AddListener(() =>
        {
            //todo: implement some logic so that when all the other players
            //have declined the trade, the tradeCanvas should close
            tradeOfferCanvas.gameObject.SetActive(false);
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
