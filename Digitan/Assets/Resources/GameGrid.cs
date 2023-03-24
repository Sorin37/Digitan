using System;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class GameGrid : NetworkBehaviour
{
    public float hexSize = 5f;

    [SerializeField] public GameObject brick;
    [SerializeField] public GameObject desert;
    [SerializeField] public GameObject grain;
    [SerializeField] public GameObject lumber;
    [SerializeField] public GameObject ore;
    [SerializeField] public GameObject wool;
    public GameObject numbersGrid;
    public GameObject[][] gameGrid;

    // Start is called before the first frame update
    void Start()
    {
        deleteLobby();

        if (brick == null || desert == null || grain == null || ore == null || wool == null)
        {
            Debug.LogError("Error: One of the prefabs is not assigned");
            return;
        }

        print("I be spawning");
    }

    public void CreateGrid()
    {
        initializeGameGrid();

        List<GameObject> hexPool = new List<GameObject> {
            brick, brick, brick,
            desert,
            grain, grain, grain, grain,
            lumber, lumber, lumber, lumber,
            ore, ore, ore,
            wool, wool, wool, wool
        };

        //Make the grid
        for (int x = 0; x <= gameGrid.Length / 2; x++)
        {
            for (int y = 0; y < gameGrid[x].Length; y++)
            {
                int randomIndex = UnityEngine.Random.Range(0, hexPool.Count);
                GameObject hex = hexPool[randomIndex];
                hexPool.RemoveAt(randomIndex);

                gameGrid[x][y] = Instantiate(
                    hex,
                    new Vector3(y * hexSize - x * hexSize / 2, 0, -x * hexSize * 3 / 4),
                    Quaternion.Euler(-90, 180, 0)
                    );

                gameGrid[x][y].GetComponent<NetworkObject>().Spawn(true);

                //gameGrid[x][y].gameObject.name = hex.name;
            }
        }

        for (int x = gameGrid.Length / 2 + 1; x < gameGrid.Length; x++)
        {
            for (int y = 0; y < gameGrid[x].Length; y++)
            {
                int randomIndex = UnityEngine.Random.Range(0, hexPool.Count);
                GameObject hex = hexPool[randomIndex];
                hexPool.RemoveAt(randomIndex);

                gameGrid[x][y] = Instantiate(
                    hex,
                    new Vector3(y * hexSize + x * hexSize / 2 - hexSize * 2, 0, -x * hexSize * 3 / 4),
                    Quaternion.Euler(-90, 180, 0)
                );

                gameGrid[x][y].GetComponent<NetworkObject>().Spawn(true);

                //gameGrid[x][y].gameObject.name = hex.name;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    void deleteLobby()
    {
        var currentLobby = GameObject.FindGameObjectsWithTag("Lobby")[0];

        Destroy(currentLobby);
    }

    [ClientRpc]
    public void PrintClientRpc(string msg)
    {
        print(msg);
    }

    private void initializeGameGrid()
    {
        gameGrid = new GameObject[5][];
        gameGrid[0] = new GameObject[3];
        gameGrid[1] = new GameObject[4];
        gameGrid[2] = new GameObject[5];
        gameGrid[3] = new GameObject[4];
        gameGrid[4] = new GameObject[3];

    }
}
