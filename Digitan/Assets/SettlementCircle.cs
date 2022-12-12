using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SettlementCircle : MonoBehaviour
{
    [SerializeField] private GameObject settlement;
    public bool isCube = false;
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
        //_renderer.material.color = Color.white;
        if (!isCube)
        {
            GameObject settlementObject = Instantiate(settlement, transform.position, Quaternion.Euler(90, 0, 0));
            settlementObject.GetComponent<RoadCircle>().isCube = true;
            Destroy(this);
            Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Settlement Circle"));
        }
    }
}
