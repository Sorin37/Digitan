using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeOfferManager : MonoBehaviour
{
    [SerializeField] private Canvas tradeOfferCanvas;
    [SerializeField] private Button acceptTradeButton;
    [SerializeField] private Button declineTradeButton;

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
}
