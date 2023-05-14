using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThiefGrid : MonoBehaviour
{
    [SerializeField] private GameObject thiefCircle;
    public GameObject[][] thiefGrid;
    private float hexSize = 5f;


    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateGrid()
    {
        thiefGrid = new GameObject[5][];
        thiefGrid[0] = new GameObject[3];
        thiefGrid[1] = new GameObject[4];
        thiefGrid[2] = new GameObject[5];
        thiefGrid[3] = new GameObject[4];
        thiefGrid[4] = new GameObject[3];

        for (int x = 0; x <= thiefGrid.Length / 2; x++)
        {
            for (int y = 0; y < thiefGrid[x].Length; y++)
            {
                Vector3 position = new Vector3(y * hexSize - x * hexSize / 2,
                                               0.1f,
                                               -x * hexSize * 3 / 4);

                thiefGrid[x][y] = Instantiate(
                    thiefCircle,
                    position,
                    Quaternion.Euler(-90, -90, 90)
                    );

                thiefGrid[x][y].gameObject.name = "Thief Circle";
                thiefGrid[x][y].gameObject.transform.parent = transform;
            }
        }


        for (int x = thiefGrid.Length / 2 + 1; x < thiefGrid.Length; x++)
        {
            for (int y = 0; y < thiefGrid[x].Length; y++)
            {
                Vector3 position = new Vector3(y * hexSize + x * hexSize / 2 - hexSize * 2,
                                0.1f,
                                -x * hexSize * 3 / 4);

                thiefGrid[x][y] = Instantiate(
                    thiefCircle,
                    position,
                    Quaternion.Euler(-90, -90, 90)
                );

                thiefGrid[x][y].gameObject.name = thiefCircle.name;
                thiefGrid[x][y].gameObject.transform.parent = transform;
            }
        }
    }
}
