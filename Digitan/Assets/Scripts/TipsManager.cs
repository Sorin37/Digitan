using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsManager : MonoBehaviour
{
    private float HexSize = 5;
    [SerializeField] private GameObject testPrefab;
    // Start is called before the first frame update
    void Start()
    {
        FindMostFrequentSpot();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FindMostFrequentSpot()
    {
        List<int> spacesPerRow = new List<int> { 3, 4, 4, 5, 5, 6, 6, 5, 5, 4, 4, 3 };

        for (int x = 0; x < spacesPerRow.Count; x++)
        {
            for (int y = 0; y < spacesPerRow[x]; y++)
            {
                GameObject go = null;
                if (x < spacesPerRow.Count / 2)
                {
                    go = Instantiate(
                        testPrefab,
                        new Vector3(
                            y * HexSize - x * HexSize / 2 + Mathf.Floor(x / 2) * HexSize / 2,
                            0.5f,
                            -Mathf.Floor(x / 2) * HexSize * 3 / 4 + HexSize / 2 - (x % 2) * HexSize / 4),
                        Quaternion.Euler(90, 0, 0)
                    );
                }
                else
                {
                    go = Instantiate(
                       testPrefab,
                       new Vector3(
                           y * HexSize + (x - spacesPerRow.Count / 2) * HexSize / 2 - Mathf.Floor(x / 2) * HexSize / 2,
                           0.5f,
                           -Mathf.Floor(x / 2) * HexSize * 3 / 4 + HexSize / 2 - (x % 2) * HexSize / 4),
                       Quaternion.Euler(90, 0, 0)
                    );
                }

                go.gameObject.name = "TestPrefab(" + (x + 1).ToString() + ", " + (y + 1).ToString() + ")";
            }
        }
    }
}
