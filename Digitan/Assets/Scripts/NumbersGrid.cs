using System.Collections.Generic;
using UnityEngine;

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
    private GameObject resourceGrid;
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
            number12 == null)
        {
            Debug.LogError("Error: One of the prefabs is not assigned");
            return;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateGrid()
    {
        hexSize = resourceGrid.GetComponent<GameGrid>().hexSize;

        numbersGrid = new GameObject[5][];
        numbersGrid[0] = new GameObject[3];
        numbersGrid[1] = new GameObject[4];
        numbersGrid[2] = new GameObject[5];
        numbersGrid[3] = new GameObject[4];
        numbersGrid[4] = new GameObject[3];

        instantiateThePieces();
    }

    private bool isRedNumberNearby(Vector3 position)
    {
        var colliders = Physics.OverlapSphere(
            position,
            hexSize / 2 + 2,
            (int)Mathf.Pow(2, LayerMask.NameToLayer("Number"))
        );

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.name == "6" || colliders[i].gameObject.name == "8")
                return true;
        }

        return false;
    }

    private void instantiateThePieces()
    {
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

        for (int x = 0; x <= numbersGrid.Length / 2; x++)
        {
            for (int y = 0; y < numbersGrid[x].Length; y++)
            {
                if (resourceGrid.GetComponent<GameGrid>().gameGrid[x][y].name == "Desert")
                    continue;

                Vector3 position = new Vector3(y * hexSize - x * hexSize / 2,
                                               0.1f,
                                               -x * hexSize * 3 / 4);

                int randomIndex = Random.Range(0, numbersPool.Count);
                GameObject number = numbersPool[randomIndex];
                numbersPool.RemoveAt(randomIndex);

                if (number.name == "6" || number.name == "8")
                {
                    if (isRedNumberNearby(position))
                    {
                        numbersPool.Add(number);
                        --y;
                        continue;
                    }
                }

                numbersGrid[x][y] = Instantiate(
                    number,
                    position,
                    Quaternion.Euler(-90, -90, 90)
                    );

                numbersGrid[x][y].transform.parent = transform;
                numbersGrid[x][y].gameObject.name = number.name;
                numbersGrid[x][y].gameObject.GetComponent<Number>().resource = resourceGrid.GetComponent<GameGrid>().gameGrid[x][y].gameObject.name;
            }
        }


        for (int x = numbersGrid.Length / 2 + 1; x < numbersGrid.Length; x++)
        {
            int tries = 0;

            for (int y = 0; y < numbersGrid[x].Length; y++)
            {
                if (resourceGrid.GetComponent<GameGrid>().gameGrid[x][y].name == "Desert")
                    continue;

                Vector3 position = new Vector3(y * hexSize + x * hexSize / 2 - hexSize * 2,
                                0.1f,
                                -x * hexSize * 3 / 4);

                int randomIndex = Random.Range(0, numbersPool.Count);
                GameObject number = numbersPool[randomIndex];
                numbersPool.RemoveAt(randomIndex);

                if (number.name == "6" || number.name == "8")
                {
                    if (isRedNumberNearby(position))
                    {
                        if (++tries == 10)
                        {
                            findAvailableSpace(x, y, position, number);
                            tries = 0;
                            continue;
                        }

                        numbersPool.Add(number);
                        --y;
                        continue;
                    }
                }

                numbersGrid[x][y] = Instantiate(
                    number,
                    position,
                    Quaternion.Euler(-90, -90, 90)
                );

                numbersGrid[x][y].transform.parent = transform;
                numbersGrid[x][y].gameObject.name = number.name;
                numbersGrid[x][y].gameObject.GetComponent<Number>().resource = resourceGrid.GetComponent<GameGrid>().gameGrid[x][y].gameObject.name;
            }
        }
    }

    void findAvailableSpace(int x, int y, Vector3 position, GameObject number)
    {
        List<(int, int)> availableSpaces= new List<(int, int)> ();

        for(int i=0; i < numbersGrid.Length; i++)
        {
            for(int j = 0; j < numbersGrid[i].Length; j++)
            {
                if (numbersGrid[i][j] != null)
                {
                    if (!isRedNumberNearby(numbersGrid[i][j].gameObject.transform.position))
                    {
                        availableSpaces.Add((i, j));
                    }
                }
            }
        }

        int randomIndex = Random.Range(0, availableSpaces.Count);
        int spaceI = availableSpaces[randomIndex].Item1;
        int spaceJ = availableSpaces[randomIndex].Item2;
        Vector3 availableSpacePosition = numbersGrid[spaceI][spaceJ].gameObject.transform.position;

        //move the old number
        numbersGrid[x][y] = Instantiate(
                    numbersGrid[spaceI][spaceJ].gameObject,
                    position,
                    Quaternion.Euler(-90, -90, 90)
        );
        numbersGrid[x][y].transform.parent = transform;
        numbersGrid[x][y].gameObject.name = numbersGrid[spaceI][spaceJ].gameObject.name;
        numbersGrid[x][y].gameObject.GetComponent<Number>().resource = numbersGrid[spaceI][spaceJ].gameObject.GetComponent<Number>().resource;

        //instantiate the new number
        Destroy(numbersGrid[spaceI][spaceJ]);
        numbersGrid[spaceI][spaceJ] = Instantiate(
                    number,
                    availableSpacePosition,
                    Quaternion.Euler(-90, -90, 90)
                );
        numbersGrid[spaceI][spaceJ].transform.parent = transform;
        numbersGrid[spaceI][spaceJ].gameObject.name = number.name;
        numbersGrid[spaceI][spaceJ].gameObject.GetComponent<Number>().resource = resourceGrid.GetComponent<GameGrid>().gameGrid[spaceJ][spaceJ].gameObject.name;
    }
}
