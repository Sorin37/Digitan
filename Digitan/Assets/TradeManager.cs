using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeManager : MonoBehaviour
{
    [SerializeField] private Canvas tradeCanvas;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button tradeBankButton;
    [SerializeField] private Button tradePlayersButton;

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
}
