using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoadCircle : MonoBehaviour
{
    private GameObject _renderer;
    [SerializeField] private GameObject road;
    public bool isOccupied = false;

    // Start is called before the first frame update
    void Start()
    {
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
            var Colliders = Physics.OverlapSphere(
                transform.position,
                1,
               (int)Mathf.Pow(2, LayerMask.NameToLayer("Unvisible Circle"))
            );

            for (int i = 0; i < Colliders.Length; i++) {
                Colliders[i].gameObject.layer = LayerMask.NameToLayer("Settlement Circle");
            }

            GameObject roadObject = Instantiate(road, transform.position, Quaternion.Euler(90, 0, 0));
            roadObject.GetComponent<RoadCircle>().isOccupied = true;
            Destroy(this);

            //make the road circles invisible
            Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Road Circle"));
        }
    }
}
