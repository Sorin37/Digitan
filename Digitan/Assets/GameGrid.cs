using Unity.VisualScripting;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    private float HexSize = 5f;

    [SerializeField] private GameObject hexTilePrefab;
    private GameObject[][] gameGrid;

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        gameGrid = new GameObject[5][];
        gameGrid[0] = new GameObject[3];
        gameGrid[1] = new GameObject[4];
        gameGrid[2] = new GameObject[5];
        gameGrid[3] = new GameObject[4];
        gameGrid[4] = new GameObject[3];
        if (hexTilePrefab == null)
        {
            Debug.LogError("Error: Hex Tile Prefab on the Game grid is not assigned");
            return;
        }

        //Make the grid
        for (int x = 0; x <= gameGrid.Length/2; x++)
        {
            for (int y = 0; y < gameGrid[x].Length; y++)
            {
                gameGrid[x][y] = Instantiate(
                    hexTilePrefab, 
                    new Vector3(y * HexSize - x * HexSize / 2, 0, -x * HexSize * 3 / 4),
                    Quaternion.Euler(90, 0, 0)
                    );
                gameGrid[x][y].transform.parent = transform;
                gameGrid[x][y].gameObject.name = "HexTile(" + x.ToString() + ", " + y.ToString() + ")";
            }
        }

        for (int x = gameGrid.Length / 2; x < gameGrid.Length; x++)
        {
            for (int y = 0; y < gameGrid[x].Length; y++)
            {
                gameGrid[x][y] = Instantiate(
                    hexTilePrefab,
                    new Vector3(y * HexSize + x * HexSize / 2 - HexSize * 2, 0, -x * HexSize * 3 / 4),
                    Quaternion.Euler(90, 0, 0)
                    );
                gameGrid[x][y].transform.parent = transform;
                gameGrid[x][y].gameObject.name = "HexTile(" + x.ToString() + ", " + y.ToString() + ")";
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
