using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RoadCircle : MonoBehaviour
{
    private GameObject _renderer;
    [SerializeField] private GameObject Road;
    private GameObject[][] roadGrid;
    public bool isOccupied = false;

    // Start is called before the first frame update
    void Start()
    {
        roadGrid = GameObject.Find("AvailableRoadsGrid").transform.GetComponent<RoadGrid>().roadGrid;
        //_renderer = GetComponent<Renderer>();

        //_renderer = GetComponent<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseDown()
    {

        if (!isOccupied)
        {
            var indexes = getIndexesOfElem(gameObject);

            if (getMyPlayer().GetComponent<StartGame>().getIsHost())
            {
                getHostPlayer().GetComponent<StartGame>().placeRoadClientRpc(indexes.x, indexes.y);
            }
            else
            {
                getHostPlayer().GetComponent<StartGame>().placeRoadServerRpc(indexes.x, indexes.y);
            }
        }
    }

    private (int x, int y) getIndexesOfElem(GameObject circle)
    {
        GameObject[] row = null;

        foreach (GameObject[] roadGridRow in roadGrid)
        {
            if (roadGridRow.Contains(circle))
            {
                row = roadGridRow;
                break;
            }
        }

        if (row != null)
        {
            return (roadGrid.ToList().IndexOf(row), row.ToList().IndexOf(circle));
        }

        return (-1, -1);
    }

    private Quaternion getRotationFromPos((int x, int y) pos)
    {
        int y = 0;

        if (pos.x % 2 == 0)
        {
            y = 60;

            if (pos.y % 2 == 1)
            {
                y *= -1;
            }
        }

        if (pos.x % 2 == 0 && pos.x > 5)
        {
            y = -60;

            if (pos.y % 2 == 1)
            {
                y *= -1;
            }
        }

        return Quaternion.Euler(-90, y, 0);
    }

    private GameObject getHostPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<StartGame>().IsOwnedByServer)
                return p;
        }

        return null;
    }
    private GameObject getMyPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<StartGame>().IsOwner)
                return p;
        }

        return null;
    }

}
