using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button settlementButton;
    [SerializeField] private Button roadButton;
    [SerializeField] private Button diceButton;
    public GameObject gameGrid;
    public GameObject player;

    private void Awake()
    {
        initSettlementButton();
        initRoadButton();
        initDiceButton();
    }

    private void initRoadButton()
    {
        roadButton.onClick.AddListener(() =>
        {
            //if (hasRoadResources())
            //{
            Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Road Circle"));
            //}
            //else
            //{
            //    print("You don't have enough resources for a road!");
            //}
        });
    }
    private void initSettlementButton()
    {
        settlementButton.onClick.AddListener(() =>
        {
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
    private void initDiceButton()
    {
        diceButton.onClick.AddListener(() =>
        {
            int dice1 = Random.Range(1, 7);
            int dice2 = Random.Range(1, 7);

            //var resourcesDict = gameGrid.GetComponent<GameGrid>().resourcesDict;
            //var playerHand = gameGrid.GetComponent<GameGrid>().playerHand;

            //if (resourcesDict.ContainsKey((dice1 + dice2).ToString()))
            //{
            //    print((dice1 + dice2).ToString() + " You got: " + String.Join(",", resourcesDict[(dice1 + dice2).ToString()].ToArray()));

            //    foreach (String resource in resourcesDict[(dice1 + dice2).ToString()])
            //    {
            //        playerHand[resource]++;
            //    }

            //    displayCards();
            //}
            //else
            //{
            //    print((dice1 + dice2).ToString() + " You got nothing!");
            //}
        });
    }
    bool hasSettlementResources()
    {
        //var playerHand = gameGrid.GetComponent<GameGrid>().playerHand;
        //if (playerHand["Brick Resource"] > 0 &&
        //    playerHand["Grain Resource"] > 0 &&
        //    playerHand["Lumber Resource"] > 0 &&
        //    playerHand["Wool Resource"] > 0)
        //{
        //    playerHand["Brick Resource"]--;
        //    playerHand["Grain Resource"]--;
        //    playerHand["Lumber Resource"]--;
        //    playerHand["Wool Resource"]--;
        //    return true;
        //}
        return false;
    }

    bool hasRoadResources()
    {
        //var playerHand = gameGrid.GetComponent<GameGrid>().playerHand;
        //if (playerHand["Brick Resource"] > 0 &&
        //    playerHand["Lumber Resource"] > 0)
        //{
        //    playerHand["Brick Resource"]--;
        //    playerHand["Lumber Resource"]--;
        //    return true;
        //}
        return false;
    }

    public void displayCards()
    {
        //var playerHand = gameGrid.GetComponent<GameGrid>().playerHand;

        //foreach (var card in playerHand)
        //{
        //    gameObject.transform.Find(card.Key.Substring(0, card.Key.IndexOf(" ")) + "Label").GetComponent<TextMeshProUGUI>().SetText("x " + card.Value.ToString());
        //}
    }
}
