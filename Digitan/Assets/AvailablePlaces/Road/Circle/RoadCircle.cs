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
        //_renderer.material.color = Color.white;
        if (!isOccupied)
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

            GameObject roadObject = Instantiate(Road,
                                                transform.position,
                                                getRotationFromPos(getIndexesOfElem(gameObject)));
            roadObject.GetComponent<RoadCircle>().isOccupied = true;

            Destroy(this.gameObject);

            //make the road circles invisible
            Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Road Circle"));
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
        int y=0;

        if (pos.x % 2 == 0)
        {
            y = 60;

            if(pos.y % 2 == 1)
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
}
