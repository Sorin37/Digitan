using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlementGrid : MonoBehaviour
{
    private float HexSize = 5f;
    [SerializeField] private GameObject circlePrefab;
    public GameObject[][] settlementGrid;
    public GameObject gameGrid;
    public bool is1stSettlementNext;
    public bool is2ndSettlementNext;

    // Awake is called before all the Start functions when the script is loaded
    void Awake()
    {
        gameGrid = GameObject.Find("GameGrid");
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (circlePrefab == null)
        {
            Debug.LogError("Error: No prefab assigned");
            return;
        }

        gameGrid.GetComponent<GameGrid>().OnGridCreated += CreateGridEvent;

        placeFirstSettlement();
    }


    // Update is called once per frame
    void Update()
    {

    }
    private void CreateGridEvent(object s, EventArgs e)
    {
        CreateGrid();

        gameGrid.GetComponent<GameGrid>().OnGridCreated -= CreateGridEvent;
    }

    void CreateGrid()
    {
        settlementGrid = new GameObject[12][];
        settlementGrid[0] = new GameObject[3];
        settlementGrid[1] = new GameObject[4];
        settlementGrid[2] = new GameObject[4];
        settlementGrid[3] = new GameObject[5];
        settlementGrid[4] = new GameObject[5];
        settlementGrid[5] = new GameObject[6];
        settlementGrid[6] = new GameObject[6];
        settlementGrid[7] = new GameObject[5];
        settlementGrid[8] = new GameObject[5];
        settlementGrid[9] = new GameObject[4];
        settlementGrid[10] = new GameObject[4];
        settlementGrid[11] = new GameObject[3];

        //Make the grid
        for (int x = 0; x < settlementGrid.Length; x++)
        {
            for (int y = 0; y < settlementGrid[x].Length; y++)
            {
                if (x < settlementGrid.Length / 2)
                {
                    settlementGrid[x][y] = Instantiate(
                        circlePrefab,
                        new Vector3(
                            y * HexSize - x * HexSize / 2 + Mathf.Floor(x / 2) * HexSize / 2,
                            0.5f,
                            -Mathf.Floor(x / 2) * HexSize * 3 / 4 + HexSize / 2 - (x % 2) * HexSize / 4),
                        Quaternion.Euler(90, 0, 0)
                    );
                }
                else
                {
                    settlementGrid[x][y] = Instantiate(
                       circlePrefab,
                       new Vector3(
                           y * HexSize + (x - settlementGrid.Length / 2) * HexSize / 2 - Mathf.Floor(x / 2) * HexSize / 2,
                           0.5f,
                           -Mathf.Floor(x / 2) * HexSize * 3 / 4 + HexSize / 2 - (x % 2) * HexSize / 4),
                       Quaternion.Euler(90, 0, 0)
                    );
                }

                settlementGrid[x][y].transform.parent = transform;
                settlementGrid[x][y].gameObject.name = "SettlementSpace(" + x.ToString() + ", " + y.ToString() + ")";
            }
        }
    }

    void placeFirstSettlement()
    {
        Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Settlement Circle"));

        is1stSettlementNext = true;
        is2ndSettlementNext = false;
    }

    private GameObject getHostPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().IsOwnedByServer)
                return p;
        }

        return null;
    }

    private GameObject getMyPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().IsOwner)
                return p;
        }

        return null;
    }
}