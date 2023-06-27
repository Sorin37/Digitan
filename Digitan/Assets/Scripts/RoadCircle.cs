using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class RoadCircle : MonoBehaviour
{
    [SerializeField] private GameObject Road;
    private GameObject settlementGrid;
    private RoadGrid availableRoadsGrid;
    private GameObject[][] roadGrid;
    public bool isOccupied = false;

    // Start is called before the first frame update
    void Start()
    {
        availableRoadsGrid = GameObject.Find("AvailableRoadsGrid").GetComponent<RoadGrid>();
        roadGrid = availableRoadsGrid.roadGrid;
        settlementGrid = GameObject.Find("AvailableSettlementGrid");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseDown()
    {
        //necessary piece of code in order to prevent clicking through UI elements
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (isOccupied)
        {
            return;
        }

        var indexes = getIndexesOfElem(gameObject, roadGrid);

        var myPlayer = getMyPlayer().GetComponent<Player>();

        turnNearbyCirclesAvailable();

        if (myPlayer.GetIsHost())
        {
            getHostPlayer().GetComponent<Player>().PlaceRoadClientRpc(
                indexes.x,
                indexes.y,
                myPlayer.color
            );
        }
        else
        {
            getHostPlayer().GetComponent<Player>().PlaceRoadServerRpc(
                indexes.x,
                indexes.y,
                myPlayer.color
            );
        }

        if (settlementGrid.GetComponent<SettlementGrid>().isStartPhase)
        {
            TurnAllStartCirclesToRoad();
            if (!settlementGrid.GetComponent<SettlementGrid>().endStartPhase)
            {
                getMyPlayer().GetComponent<Player>().PlacedServerRpc();
            }
            else
            {
                getHostPlayer().GetComponent<Player>().EndStartPhaseClientRpc();
                getHostPlayer().GetComponent<Player>().currentPlayerTurn.Value = 0;
            }
        }

        getMyPlayer().GetComponent<Player>().nrOfPlacedRoads++;
    }

    private (int x, int y) getIndexesOfElem(GameObject circle, GameObject[][] grid)
    {
        GameObject[] row = null;

        foreach (GameObject[] roadGridRow in grid)
        {
            if (roadGridRow.Contains(circle))
            {
                row = roadGridRow;
                break;
            }
        }

        if (row != null)
        {
            return (grid.ToList().IndexOf(row), row.ToList().IndexOf(circle));
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


    private void turnNearbyCirclesAvailable()
    {
        var colliders = Physics.OverlapSphere(
                transform.position,
                2,
               (int)Mathf.Pow(2, LayerMask.NameToLayer("Unvisible Circle"))
            );

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.GetComponent<SettlementCircle>() != null)
            {
                if (!colliders[i].gameObject.GetComponent<SettlementCircle>().isTooClose)
                {
                    colliders[i].gameObject.layer = LayerMask.NameToLayer("Settlement Circle");
                }
            }
            else
            {
                colliders[i].gameObject.layer = LayerMask.NameToLayer("Road Circle");
            }
        }
    }

    private void TurnAllStartCirclesToRoad()
    {

        for (int i = 0; i < roadGrid.Length; i++)
        {
            for (int j = 0; j < roadGrid[i].Length; j++)
            {
                if (roadGrid[i][j] != null)
                {
                    if (roadGrid[i][j].layer == LayerMask.NameToLayer("Start Circle"))
                    {
                        roadGrid[i][j].layer = LayerMask.NameToLayer("Road Circle");
                    }
                }
            }
        }
    }
}
