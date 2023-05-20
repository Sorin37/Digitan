using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StealManager : MonoBehaviour
{
    [SerializeField] private GameObject StealBoard;
    [SerializeField] private GameObject PlayerDetailsPrefab;

    public class PlayerDetails
    {
        public ulong id;
        public string name;
        public Color color;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DisplayPlayersToStealFrom(List<PlayerDetails> players)
    {
        int i = 0;
        foreach (PlayerDetails player in players)
        {
            GameObject playerDetails = Instantiate(
                    PlayerDetailsPrefab,
                    new Vector3(0, i * -150 + 225, 0),
                    StealBoard.transform.rotation
                );

            playerDetails.transform.Find("PlayerName")
            .gameObject.GetComponent<TextMeshProUGUI>()
                    .text = player.name;

            playerDetails.transform.Find("ColorPanel")
            .gameObject.GetComponent<Image>().color = player.color;

            playerDetails.transform.Find("Button")
                         .gameObject.GetComponent<Button>()
                         .onClick.AddListener(() =>
                         {
                             GetHostPlayer().GetComponent<Player>().StealServerRpc(player.id, new Unity.Netcode.ServerRpcParams());
                             transform.parent.gameObject.SetActive(false);
                         });

            playerDetails.transform.SetParent(StealBoard.transform, false);

            i++;
        }

    }

    private GameObject GetHostPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().IsOwnedByServer)
                return p;
        }

        return null;
    }
}
