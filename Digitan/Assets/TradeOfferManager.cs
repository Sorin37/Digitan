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
            //todo: implement some logic so that you'll receive the resources
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

    public void Draw(int giveBrick, int giveGrain, int giveLumber, int giveOre, int giveWool, 
                     int getBrick, int getGrain, int getLumber, int getOre, int getWool)
    {
        giveBrickLabel.text = "x " + giveBrick.ToString();
        giveGrainLabel.text = "x " + giveGrain.ToString();
        giveLumberLabel.text = "x " + giveLumber.ToString();
        giveOreLabel.text = "x " + giveOre.ToString();
        giveWoolLabel.text = "x " + giveWool.ToString();

        getBrickLabel.text = "x " + getBrick.ToString();
        getGrainLabel.text = "x " + getGrain.ToString();
        getLumberLabel.text = "x " + getLumber.ToString();
        getOreLabel.text = "x " + getOre.ToString();
        getWoolLabel.text = "x " + getWool.ToString();
    }
}
