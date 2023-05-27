using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YearOfPlentyManager : MonoBehaviour
{
    [SerializeField] private Button brickButton;
    [SerializeField] private Button grainButton;
    [SerializeField] private Button lumberButton;
    [SerializeField] private Button oreButton;
    [SerializeField] private Button woolButton;
    private int nrOfClickedCards = 0;

    // Start is called before the first frame update
    void Start()
    {
        brickButton.onClick.AddListener(() =>
        {
            nrOfClickedCards++;
            GetMyPlayer().playerHand["Brick Resource"]++;
            GetMyPlayer().UpdateHand();

            if (nrOfClickedCards == 2)
            {
                transform.parent.gameObject.SetActive(false);
                nrOfClickedCards = 0;
            }
        });

        grainButton.onClick.AddListener(() =>
        {
            nrOfClickedCards++;
            GetMyPlayer().playerHand["Grain Resource"]++;
            GetMyPlayer().UpdateHand();

            if (nrOfClickedCards == 2)
            {
                transform.parent.gameObject.SetActive(false);
                nrOfClickedCards = 0;
            }
        });

        lumberButton.onClick.AddListener(() =>
        {
            nrOfClickedCards++;
            GetMyPlayer().playerHand["Lumber Resource"]++;
            GetMyPlayer().UpdateHand();

            if (nrOfClickedCards == 2)
            {
                transform.parent.gameObject.SetActive(false);
                nrOfClickedCards = 0;
            }
        });

        oreButton.onClick.AddListener(() =>
        {
            nrOfClickedCards++;
            GetMyPlayer().playerHand["Ore Resource"]++;
            GetMyPlayer().UpdateHand();

            if (nrOfClickedCards == 2)
            {
                transform.parent.gameObject.SetActive(false);
                nrOfClickedCards = 0;
            }
        });

        woolButton.onClick.AddListener(() =>
        {
            nrOfClickedCards++;
            GetMyPlayer().playerHand["Wool Resource"]++;
            GetMyPlayer().UpdateHand();

            if (nrOfClickedCards == 2)
            {
                transform.parent.gameObject.SetActive(false);
                nrOfClickedCards = 0;
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Player GetMyPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().IsOwner)
                return p.GetComponent<Player>();
        }

        return null;
    }
}
