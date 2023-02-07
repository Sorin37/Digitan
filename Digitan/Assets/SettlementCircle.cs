using System;
using System.Collections;
using System.Collections.Generic;
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

        if (settlementGrid.is1stSettlementNext || settlementGrid.is2ndSettlementNext)
        {
            GameObject[][] grid = transform.parent.gameObject.GetComponent<SettlementGrid>().settlementGrid;

            //turns the settlement circle invisible after the 2nd settlement
            if (settlementGrid.is2ndSettlementNext)
            {
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

        if (!isOccupied)
        {
            var colliders = Physics.OverlapSphere(
                transform.position,
                2.5f,
               (int)(Mathf.Pow(2, LayerMask.NameToLayer("Unvisible Circle")) +
               Mathf.Pow(2, LayerMask.NameToLayer("Settlement Circle")))
            );

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.GetComponent<SettlementCircle>() != null)
                {
                    colliders[i].gameObject.GetComponent<SettlementCircle>().isTooClose = true;
                    colliders[i].gameObject.layer = LayerMask.NameToLayer("Unvisible Circle");
                }
            }

            GameObject settlementObject = Instantiate(settlement, transform.position, Quaternion.Euler(90, 0, 0));

            //todo: change to settlement circle
            settlementObject.GetComponent<RoadCircle>().isOccupied = true;
            Destroy(this.gameObject);

            Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Settlement Circle"));

            addResourcesToDict();
        }

    }

    private void addResourcesToDict()
    {
        var colliders = Physics.OverlapSphere(
            transform.position,
            2.5f,
            (int)Mathf.Pow(2, LayerMask.NameToLayer("Number"))
        );

        var resourcesDict = gameGrid.GetComponent<GameGrid>().resourcesDict;

        foreach (var collider in colliders)
        {
            if (resourcesDict.ContainsKey(collider.gameObject.name))
            {
                resourcesDict[collider.gameObject.name].Add(collider.gameObject.GetComponent<Number>().resource);
            }
            else
            {
                resourcesDict[collider.gameObject.name] = new List<String>() { collider.gameObject.GetComponent<Number>().resource };
            }

            //first settlements stuff
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
            var playerHand = gameGrid.GetComponent<GameGrid>().playerHand;
            playerHand[resource]++;

            Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Settlement Circle"));
        }

        if (settlementGrid.is2ndSettlementNext)
        {
            var playerHand = gameGrid.GetComponent<GameGrid>().playerHand;
            playerHand[resource]++;
        }
    }
}
