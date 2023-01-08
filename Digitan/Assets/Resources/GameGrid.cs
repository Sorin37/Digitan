using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public float hexSize = 5f;

    [SerializeField] private GameObject brick;
    [SerializeField] private GameObject desert;
    [SerializeField] private GameObject grain;
    [SerializeField] private GameObject lumber;
    [SerializeField] private GameObject ore;
    [SerializeField] private GameObject wool;
    [SerializeField] private GameObject numbersGrid;
    public GameObject[][] gameGrid;
    public Dictionary<String, List<String>> resourcesDict;
    public Dictionary<String, int> playerHand;


    // Start is called before the first frame update
    void Start()
    {
        if (brick == null || desert == null || grain == null || ore == null || wool == null || numbersGrid == null)
        {
            Debug.LogError("Error: One of the prefabs is not assigned");
            return;
        }

        resourcesDict = new Dictionary<String, List<String>>();
        initializePlayerHand();
        CreateGrid();
        numbersGrid.GetComponent<NumbersGrid>().CreateGrid();
    }

    private void CreateGrid()
    {
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

                gameGrid[x][y].transform.parent = transform;
                gameGrid[x][y].gameObject.name = hex.name;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void initializePlayerHand()
    {
        playerHand = new Dictionary<string, int>();
        playerHand["Brick Resource"] = 0;
        playerHand["Grain Resource"] = 0;
        playerHand["Lumber Resource"] = 0;
        playerHand["Ore Resource"] = 0;
        playerHand["Wool Resource"] = 0;
    }
}
