using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button settlementButton;
    [SerializeField] private Button roadButton;

    private void Awake()
    {
        settlementButton.onClick.AddListener(() =>
        {
            Debug.Log("xd");
        });
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
