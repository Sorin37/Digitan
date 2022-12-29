using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Assertions;

public class NumbersGrid : MonoBehaviour
{
    private float hexSize;

    [SerializeField] private GameObject number2;
    [SerializeField] private GameObject number3;
    [SerializeField] private GameObject number4;
    [SerializeField] private GameObject number5;
    [SerializeField] private GameObject number6;
    [SerializeField] private GameObject number8;
    [SerializeField] private GameObject number9;
    [SerializeField] private GameObject number10;
    [SerializeField] private GameObject number11;
    [SerializeField] private GameObject number12;
    [SerializeField] private GameObject resourceGrid;
    public GameObject[][] numbersGrid;


    // Start is called before the first frame update
    void Start()
    {
        if (number2 == null ||
            number3 == null ||
            number4 == null ||
            number5 == null ||
            number6 == null ||
            number8 == null ||
            number9 == null ||
            number10 == null ||
            number11 == null ||
            number12 == null ||
            resourceGrid == null)
        {
            Debug.LogError("Error: One of the prefabs is not assigned");
            return;
        }
        else
        {
            hexSize = resourceGrid.GetComponent<GameGrid>().hexSize;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateGrid()
    {
        numbersGrid = new GameObject[5][];
        numbersGrid[0] = new GameObject[3];
        numbersGrid[1] = new GameObject[4];
        numbersGrid[2] = new GameObject[5];
        numbersGrid[3] = new GameObject[4];
        numbersGrid[4] = new GameObject[3];

        List<GameObject> numbersPool = new List<GameObject> {
            number2,
            number3,  number3,
            number4,  number4,
            number5,  number5,
            number6,  number6,
            number8,  number8,
            number9,  number9,
            number10, number10,
            number11, number11,
            number12
        };

        //Make the grid
        for (int x = 0; x <= numbersGrid.Length / 2; x++)
        {
            for (int y = 0; y < numbersGrid[x].Length; y++)
            {
                if (resourceGrid.GetComponent<GameGrid>().gameGrid[x][y].name == "Desert")
                    continue;

                int randomIndex = Random.Range(0, numbersPool.Count);
                GameObject number = numbersPool[randomIndex];
                numbersPool.RemoveAt(randomIndex);

                numbersGrid[x][y] = Instantiate(
                    number,
                    new Vector3(y * hexSize - x * hexSize / 2,
                                0.1f,
                                -x * hexSize * 3 / 4),
                    Quaternion.Euler(-90, -90, 90)
                    );

                numbersGrid[x][y].transform.parent = transform;
                numbersGrid[x][y].gameObject.name = "NumberPiece(" + x.ToString() + ", " + y.ToString() + ")";
            }
        }

        for (int x = numbersGrid.Length / 2 + 1; x < numbersGrid.Length; x++)
        {
            for (int y = 0; y < numbersGrid[x].Length; y++)
            {
                if (resourceGrid.GetComponent<GameGrid>().gameGrid[x][y].name == "Desert")
                    continue;

                int randomIndex = Random.Range(0, numbersPool.Count);
                GameObject number = numbersPool[randomIndex];
                numbersPool.RemoveAt(randomIndex);

                numbersGrid[x][y] = Instantiate(
                    number,
                    new Vector3(y * hexSize + x * hexSize / 2 - hexSize * 2,
                                0.1f,
                                -x * hexSize * 3 / 4),
                    Quaternion.Euler(-90, -90, 90)
                );

                numbersGrid[x][y].transform.parent = transform;
                numbersGrid[x][y].gameObject.name = "NumberPiece(" + x.ToString() + ", " + y.ToString() + ")";
            }
        }
    }
}
