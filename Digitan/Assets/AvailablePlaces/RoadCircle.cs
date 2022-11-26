using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadCircle : MonoBehaviour
{
    private Renderer _renderer;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()
    {
        _renderer.material.color = Color.white;
    }
}
