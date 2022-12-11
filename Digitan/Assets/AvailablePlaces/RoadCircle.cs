using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoadCircle : MonoBehaviour
{
    private GameObject _renderer;
    [SerializeField] private GameObject road;
    public bool isCube = false;
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
        if (!isCube)
        {
            GameObject pareri = Instantiate(road, transform.position, Quaternion.Euler(90, 0, 0));
            pareri.GetComponent<RoadCircle>().isCube = true;
            Destroy(this);
        }
    }
}
