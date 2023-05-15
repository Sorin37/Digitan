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
        GameObject playerDetails = Instantiate(
                PlayerDetailsPrefab,
                new Vector3(0, 0 * -150 + 225, 0),
                StealBoard.transform.rotation
            );

        playerDetails.transform.Find("PlayerName")
        .gameObject.GetComponent<TextMeshProUGUI>()
                .text = "xdddddddddd";

        playerDetails.transform.Find("Button")
                     .gameObject.GetComponent<Button>()
                     .onClick.AddListener(() =>
                     {
                         print("less go");
                     });

        playerDetails.transform.SetParent(StealBoard.transform, false);

    }
}
