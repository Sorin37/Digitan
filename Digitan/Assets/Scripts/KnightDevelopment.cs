using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KnightDevelopment : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject oneDevelopmentPrefab;
    [SerializeField] private GameObject notYourTurnPrefab;
    [SerializeField] private GameObject newDevelopmentPrefab;

    // Start is called before the first frame update
    void Start()
    {
        InitializeButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializeButton()
    {
        button.onClick.AddListener(() =>
        {
            var myPlayer = GetMyPlayer();

            if (!IsMyTurn())
            {
                var message = Instantiate(notYourTurnPrefab, button.transform);
                message.GetComponent<RedMessage>().SetStartPosition(button.transform);
                return;
            }

            if (myPlayer.playedDevelopmentThisRound)
            {
                var message = Instantiate(oneDevelopmentPrefab, button.transform);
                message.GetComponent<RedMessage>().SetStartPosition(button.transform);
                return;
            }

            if (gameObject.transform.parent.transform.childCount == 1)
            {
                if(GetMyPlayer().boughtDevelopmentsThisRound.Contains("Knight"))
                {
                    var message = Instantiate(newDevelopmentPrefab, button.transform);
                    message.GetComponent<RedMessage>().SetStartPosition(button.transform);
                    return;
                }
            }

            myPlayer.playedDevelopmentThisRound = true;

            var player = GetHostPlayer();
            RemoveDevelopment("KnightDeck");
            player.UsedKnightServerRpc(new Unity.Netcode.ServerRpcParams());
            player.DisplayThiefCirclesServerRpc();
        });
    }

    private void RemoveDevelopment(string deckName)
    {
        //find the deck
        var deckGroup = Resources.FindObjectsOfTypeAll<DeckGroup>()[0];

        GameObject deck = null;

        foreach (Transform child in deckGroup.transform)
        {
            if (child.gameObject.name == deckName)
            {
                deck = child.gameObject;
                break;
            }
        }

        if (deck == null)
        {
            print("Didn't find the " + deckName + " after touching the card");
            return;
        }

        //remove the card
        Destroy(gameObject);

        //resize the deck
        var deckRectTransform = deck.transform as RectTransform;

        float width = 0;

        if (deckRectTransform.sizeDelta.x == 100)
        {
            width = -120;
        }
        else if (deckRectTransform.sizeDelta.x > 100)
        {
            width = -25;
        }

        deckRectTransform.sizeDelta = new Vector2(deckRectTransform.sizeDelta.x + width, deckRectTransform.sizeDelta.y);
    }

    private Player GetHostPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().IsOwnedByServer)
                return p.GetComponent<Player>();
        }

        return null;
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

    private bool IsMyTurn()
    {
        return GetHostPlayer().currentPlayerTurn.Value == (int)NetworkManager.Singleton.LocalClientId;
    }
}
