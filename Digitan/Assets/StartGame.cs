using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class StartGame : NetworkBehaviour
{
    [SerializeField] private GameObject gameGrid;
    private GameObject xd;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }



    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;
        if (Input.GetKeyUp(KeyCode.W))
        {
            FirstStepTowardsSuccessServerRpc();
        }
    }

    [ServerRpc]
    public void FirstStepTowardsSuccessServerRpc()
    {
        xd = Instantiate(gameGrid);
        xd.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc]
    public void SecondServerRpc()
    {
        var gameGrid = xd.GetComponent<GameGrid>().gameGrid;
        var brick = xd.GetComponent<GameGrid>().brick;
        var desert = xd.GetComponent<GameGrid>().desert;
        var grain = xd.GetComponent<GameGrid>().grain;
        var lumber = xd.GetComponent<GameGrid>().lumber;
        var ore = xd.GetComponent<GameGrid>().ore;
        var wool = xd.GetComponent<GameGrid>().wool;
        var hexSize = xd.GetComponent<GameGrid>().hexSize;

        gameGrid = new GameObject[5][];
        gameGrid[0] = new GameObject[3];
        gameGrid[1] = new GameObject[4];
        gameGrid[2] = new GameObject[5];
        gameGrid[3] = new GameObject[4];
        gameGrid[4] = new GameObject[3];



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

                gameGrid[x][y].transform.parent = transform;
                gameGrid[x][y].gameObject.name = hex.name;
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

                gameGrid[x][y].transform.parent = transform;
                gameGrid[x][y].gameObject.name = hex.name;
            }
        }
    }

}
