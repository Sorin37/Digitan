using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class RoadGrid : MonoBehaviour
{
    private float HexSize = 5f;
    [SerializeField] private GameObject circlePrefab;
    private GameObject[][] roadGrid;

    // Start is called before the first frame update
    void Start()
    {
        if (circlePrefab == null)
        {
            Debug.LogError("Error: No prefab assigned");
            return;
        }

        roadGrid = new GameObject[11][];
        roadGrid[0] = new GameObject[6];
        roadGrid[1] = new GameObject[4];
        roadGrid[2] = new GameObject[8];
        roadGrid[3] = new GameObject[5];
        roadGrid[4] = new GameObject[10];
        roadGrid[5] = new GameObject[6];
        roadGrid[6] = new GameObject[10];
        roadGrid[7] = new GameObject[5];
        roadGrid[8] = new GameObject[8];
        roadGrid[9] = new GameObject[4];
        roadGrid[10] = new GameObject[6];

        //Make the grid
        for (int x = 0; x < roadGrid.Length; x = x + 2)
        {
            for (int y = 0; y < roadGrid[x].Length; y++)
            {
                if (x < roadGrid.Length / 2)
                {
                    roadGrid[x][y] = Instantiate(
                        circlePrefab,
                        new Vector3(y * HexSize / 2 - HexSize / 4 * (x + 1), 0.5f, -x * HexSize * 0.375f + HexSize / 3),
                        Quaternion.Euler(90, 0, 0)
                    );
                }
                else
                {
                    roadGrid[x][y] = Instantiate(
                        circlePrefab,
                        new Vector3(y * HexSize / 2 + HexSize / 4 * (x - roadGrid.Length), 0.5f, -x * HexSize * 0.375f + HexSize / 3),
                        Quaternion.Euler(90, 0, 0)
                    );
                }

                roadGrid[x][y].transform.parent = transform;
                roadGrid[x][y].gameObject.name = "RoadSpace(" + x.ToString() + ", " + y.ToString() + ")";
            }
        }

        for (int x = 1; x < roadGrid.Length; x = x + 2)
        {
            for (int y = 0; y < roadGrid[x].Length; y++)
            {
                if (x < roadGrid.Length / 2)
                {
                    roadGrid[x][y] = Instantiate(
                        circlePrefab,
                        new Vector3(y * HexSize - HexSize / 4 * (x + 1), 0.5f, -x * HexSize * 0.375f + HexSize / 3),
                        Quaternion.Euler(90, 0, 0)
                        );

                    roadGrid[x][y].transform.parent = transform;
                    roadGrid[x][y].gameObject.name = "RoadSpace(" + x.ToString() + ", " + y.ToString() + ")";
                }
                else
                {
                    roadGrid[x][y] = Instantiate(
                        circlePrefab,
                        new Vector3(y * HexSize + HexSize / 4 * (x - 11), 0.5f, -x * HexSize * 0.375f + HexSize / 3),
                        Quaternion.Euler(90, 0, 0)
                    );

                    roadGrid[x][y].transform.parent = transform;
                    roadGrid[x][y].gameObject.name = "RoadSpace(" + x.ToString() + ", " + y.ToString() + ")";
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
