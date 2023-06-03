using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsManager : MonoBehaviour
{
    private float HexSize = 5;

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

        int max = 0;

        for (int x = 0; x < spacesPerRow.Count; x++)
        {
            for (int y = 0; y < spacesPerRow[x]; y++)
            {
                int value = 0;

                if (x < spacesPerRow.Count / 2)
                {
                    value = CollidersValue(new Vector3(
                        y * HexSize - x * HexSize / 2 + Mathf.Floor(x / 2) * HexSize / 2,
                        0.5f,
                        -Mathf.Floor(x / 2) * HexSize * 3 / 4 + HexSize / 2 - (x % 2) * HexSize / 4)
                    );
                }
                else
                {
                    value = CollidersValue(new Vector3(
                           y * HexSize + (x - spacesPerRow.Count / 2) * HexSize / 2 - Mathf.Floor(x / 2) * HexSize / 2,
                           0.5f,
                           -Mathf.Floor(x / 2) * HexSize * 3 / 4 + HexSize / 2 - (x % 2) * HexSize / 4)
                    );
                }

                max = max < value ? value : max;
            }
        }

        print("Cel mai bun spot are: " + max);
    }

    private int CollidersValue(Vector3 pos)
    {

        int value = 0;

        var colliders = Physics.OverlapSphere(
            pos,
            2.5f,
            (int)Mathf.Pow(2, LayerMask.NameToLayer("Number"))
        );

        foreach (var collider in colliders)
        {
            value += collider.GetComponent<Number>().frequency;
        }

        return value;
    }
}
