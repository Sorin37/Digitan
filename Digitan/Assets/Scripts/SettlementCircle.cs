using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms;
using UnityEngine.UIElements;

public class SettlementCircle : MonoBehaviour
{
    [SerializeField] private GameObject cityPlace;
    public bool isOccupied = false;
    public bool isTooClose = false;
    public GameObject gameGrid;

    // Start is called before the first frame update
    void Start()
    {
        gameGrid = GameObject.Find("GameGrid");

        if (cityPlace == null || gameGrid == null)
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
        //necessary piece of code in order to prevent clicking through UI elements
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

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
            GiveTradePort();

            TurnCloseRoadsAvailable();

            var indexes = getIndexesOfElem(gameObject, settlementGrid.settlementGrid);

            var myPlayer = GetMyPlayer().GetComponent<Player>();

            if (GetMyPlayer().GetComponent<Player>().GetIsHost())
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

            InstantiateCityPlace();

            addResourcesToDict();
            GetMyPlayer().GetComponent<Player>().UpdateHand();
        }

        if (settlementGrid.isStartPhase)
        {
            var colliders = Physics.OverlapSphere(
                transform.position,
                1,
                (int)Mathf.Pow(2, LayerMask.NameToLayer("Road Circle"))
            );

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].gameObject.layer = LayerMask.NameToLayer("Start Circle");
            }

            Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Start Circle"));
        }
    }

    private void addResourcesToDict()
    {
        var colliders = Physics.OverlapSphere(
            transform.position,
            2.5f,
            (int)Mathf.Pow(2, LayerMask.NameToLayer("Number"))
        );

        var settlementGrid = transform.parent.gameObject.GetComponent<SettlementGrid>();

        foreach (var collider in colliders)
        {
            var resourcesDict = GetMyPlayer().GetComponent<Player>().resourcesDict;
            resourcesDict[collider.gameObject.name].Add(collider.gameObject.GetComponent<Number>().resource);

            //first two settlements stuff
            if (settlementGrid.isStartPhase)
            {
                GiveResourcesForStartPhase(collider.gameObject.GetComponent<Number>().resource);
            }
        }

        //so that they will get it the second time
        settlementGrid.canGetStartPhaseResources = true;
    }

    void GiveResourcesForStartPhase(string resource)
    {
        var settlementGrid = transform.parent.gameObject.GetComponent<SettlementGrid>();

        if (settlementGrid.canGetStartPhaseResources)
        {
            GetMyPlayer().GetComponent<Player>().playerHand[resource]++;
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
    private GameObject GetMyPlayer()
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

    private void GiveTradePort()
    {
        var tradeDict = GetMyPlayer().GetComponent<Player>().tradeDict;

        var colliders = Physics.OverlapSphere(
            transform.position,
            1,
           (int)Mathf.Pow(2, LayerMask.NameToLayer("Port"))
        );

        foreach(var collider in colliders)
        {
            tradeDict[collider.gameObject.GetComponent<Port>().type] = true;
        }
    }

    private void InstantiateCityPlace()
    {
        var city = Instantiate(
                    cityPlace,
                    transform.position,
                    Quaternion.Euler(0, 0, 0)
                );
    }
}
