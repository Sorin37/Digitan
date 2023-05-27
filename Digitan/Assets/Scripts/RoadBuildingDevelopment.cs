using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoadBuildingDevelopment : MonoBehaviour
{
    [SerializeField] private Button button;

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
            RemoveDevelopment("RoadBuildingDeck");
            GameObject.Find("AvailableRoadsGrid").GetComponent<RoadGrid>().usedRoadBuilding = true;
            Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Road Circle"));
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
}
