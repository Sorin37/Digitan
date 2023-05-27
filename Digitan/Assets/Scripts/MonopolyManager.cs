using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MonopolyManager : MonoBehaviour
{
    [SerializeField] private Button brickButton;
    [SerializeField] private Button grainButton;
    [SerializeField] private Button lumberButton;
    [SerializeField] private Button oreButton;
    [SerializeField] private Button woolButton;

    // Start is called before the first frame update
    void Start()
    {
        brickButton.onClick.AddListener(() =>
        {
            GetHostPlayer().MonopolyServerRpc("Brick Resource", new ServerRpcParams());

            transform.parent.gameObject.SetActive(false);
        });

        grainButton.onClick.AddListener(() =>
        {
            GetHostPlayer().MonopolyServerRpc("Grain Resource", new ServerRpcParams());

            transform.parent.gameObject.SetActive(false);
        });

        lumberButton.onClick.AddListener(() =>
        {
            GetHostPlayer().MonopolyServerRpc("Lumber Resource", new ServerRpcParams());

            transform.parent.gameObject.SetActive(false);
        });

        oreButton.onClick.AddListener(() =>
        {
            GetHostPlayer().MonopolyServerRpc("Ore Resource", new ServerRpcParams());

            transform.parent.gameObject.SetActive(false);
        });

        woolButton.onClick.AddListener(() =>
        {
            GetHostPlayer().MonopolyServerRpc("Wool Resource", new ServerRpcParams());

            transform.parent.gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {

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
}
