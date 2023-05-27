using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KnightDevelopment : MonoBehaviour
{
    [SerializeField] private Button button;

    // Start is called before the first frame update
    void Start()
    {
        print("K am un cavaler");
        InitializeButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitializeButton()
    {
        button.onClick.AddListener(() =>
        {
            print("I am touched");
        });
    }
}
