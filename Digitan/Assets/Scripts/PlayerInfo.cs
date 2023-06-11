using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private GameObject color;
    [SerializeField] private TextMeshProUGUI nickname;
    [SerializeField] private TextMeshProUGUI numberOfCards;
    [SerializeField] private TextMeshProUGUI numberOfDevelopments;
    [SerializeField] private TextMeshProUGUI victoryPoints;
    [SerializeField] private GameObject largestArmy;
    [SerializeField] private GameObject longestRoad;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetInfo(ulong playerId)
    {
        color.GetComponent<Image>().color = GetMyPlayer().IdToColor(playerId);

        var targetPlayer = GetPlayerWithId(playerId);

        nickname.text = targetPlayer.nickName.Value.ToString();

        numberOfCards.text = targetPlayer.numberOfCards.Value.ToString();

        numberOfDevelopments.text = targetPlayer.numberOfDevelopments.Value.ToString();

        if(playerId == GetMyPlayer().OwnerClientId)
        {
            victoryPoints.text = targetPlayer.nrOfVictoryPoints.Value.ToString();
        }
        else
        {
            victoryPoints.gameObject.SetActive(false);
        }

        if(targetPlayer.hasLargestArmy.Value)
        {
            largestArmy.SetActive(true);
        }

        if (targetPlayer.hasLongestRoad.Value)
        {
            longestRoad.SetActive(true);
        }
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

    private Player GetPlayerWithId(ulong id)
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().OwnerClientId == id)
                return p.GetComponent<Player>();
        }

        return null;
    }
}
