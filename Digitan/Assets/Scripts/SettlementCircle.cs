using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SocialPlatforms;

public class SettlementCircle : MonoBehaviour
{
    [SerializeField] private GameObject settlement;
    public bool isOccupied = false;
    public bool isTooClose = false;
    public GameObject gameGrid;

    // Start is called before the first frame update
    void Start()
    {
        gameGrid = GameObject.Find("GameGrid");

        if (settlement == null || gameGrid == null)
        {
            Debug.LogError("Error: One of the prefabs is not assigned");
            return;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseDown()
    {
        var settlementGrid = transform.parent.gameObject.GetComponent<SettlementGrid>();

        //if (settlementGrid.is1stSettlementNext || settlementGrid.is2ndSettlementNext)
        //{

        //    //turns the settlement circle invisible after the 2nd settlement
        //    if (settlementGrid.is2ndSettlementNext)
        //    {
        //        TurnSettlementsInvisible();
        //    }

        //}

        if (!isOccupied)
        {
            TurnCloseRoadsAvailable();

            var indexes = getIndexesOfElem(gameObject, settlementGrid.settlementGrid);

            var myPlayer = getMyPlayer().GetComponent<Player>();

            if (getMyPlayer().GetComponent<Player>().GetIsHost())
            {
                getHostPlayer().GetComponent<Player>().placeSettlementClientRpc(
                    indexes.x, 
                    indexes.y,
                    myPlayer.color
                );
            }
            else
            {
                getHostPlayer().GetComponent<Player>().placeSettlementServerRpc(
                    indexes.x,
                    indexes.y,
                    myPlayer.color
                );
            }

            //addResourcesToDict();
        }

    }

    private void addResourcesToDict()
    {
        var colliders = Physics.OverlapSphere(
            transform.position,
            2.5f,
            (int)Mathf.Pow(2, LayerMask.NameToLayer("Number"))
        );

        //var resourcesDict = gameGrid.GetComponent<GameGrid>().resourcesDict;

        foreach (var collider in colliders)
        {
            //if (resourcesDict.ContainsKey(collider.gameObject.name))
            //{
            //    resourcesDict[collider.gameObject.name].Add(collider.gameObject.GetComponent<Number>().resource);
            //}
            //else
            //{
            //    resourcesDict[collider.gameObject.name] = new List<String>() { collider.gameObject.GetComponent<Number>().resource };
            //}

            //first two settlements stuff
            giveResourcesForTheFirstSettlements(collider.gameObject.GetComponent<Number>().resource);

        }
        var settlementGrid = transform.parent.gameObject.GetComponent<SettlementGrid>();

        if (settlementGrid.is1stSettlementNext)
        {
            settlementGrid.is1stSettlementNext = false;
            settlementGrid.is2ndSettlementNext = true;
        }
        else if (settlementGrid.is2ndSettlementNext)
        {
            settlementGrid.is2ndSettlementNext = false;
        }
    }
    void giveResourcesForTheFirstSettlements(String resource)
    {
        var settlementGrid = transform.parent.gameObject.GetComponent<SettlementGrid>();

        if (settlementGrid.is1stSettlementNext)
        {
            //var playerHand = gameGrid.GetComponent<GameGrid>().playerHand;
            //playerHand[resource]++;

            Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Settlement Circle"));
        }

        if (settlementGrid.is2ndSettlementNext)
        {
            //var playerHand = gameGrid.GetComponent<GameGrid>().playerHand;
            //playerHand[resource]++;
        }
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

    private void TurnSettlementsInvisible()
    {
        GameObject[][] grid = transform.parent.gameObject.GetComponent<SettlementGrid>().settlementGrid;

        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[i].Length; j++)
            {
                if (grid[i][j] != null)
                {
                    grid[i][j].layer = LayerMask.NameToLayer("Unvisible Circle");
                }
            }
        }
    }

    private void TurnCloseRoadsAvailable()
    {
        var colliders = Physics.OverlapSphere(
            transform.position,
            1,
           (int)Mathf.Pow(2, LayerMask.NameToLayer("Unvisible Circle"))
        );

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].gameObject.layer = LayerMask.NameToLayer("Road Circle");
        }
    }
}
