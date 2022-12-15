using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SettlementCircle : MonoBehaviour
{
    [SerializeField] private GameObject settlement;
    public bool isOccupied = false;
    public bool isTooClose = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()
    {
        if (transform.parent.gameObject.GetComponent<SettlementGrid>().isFirstSettlement)
        {
            GameObject[][] grid = transform.parent.gameObject.GetComponent<SettlementGrid>().settlementGrid;

            for(int i = 0; i < grid.Length; i++)
            {
                for(int j = 0; j < grid[i].Length; j++)
                {
                    grid[i][j].layer = LayerMask.NameToLayer("Unvisible Circle");
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
                    print(colliders[i].gameObject.name);
                    colliders[i].gameObject.GetComponent<SettlementCircle>().isTooClose = true;
                    colliders[i].gameObject.layer = LayerMask.NameToLayer("Unvisible Circle");
                }
            }

            GameObject settlementObject = Instantiate(settlement, transform.position, Quaternion.Euler(90, 0, 0));
            settlementObject.GetComponent<RoadCircle>().isOccupied = true;
            Destroy(this);

            Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Settlement Circle"));

            transform.parent.gameObject.GetComponent<SettlementGrid>().isFirstSettlement = false;
        }
    }
}
