using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button settlementButton;
    [SerializeField] private Button roadButton;
    [SerializeField] private Button diceButton;
    public GameObject gameGrid;

    private void Awake()
    {

        settlementButton.onClick.AddListener(() =>
        {
            Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Settlement Circle"));
        });

        roadButton.onClick.AddListener(() =>
        {
            Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("Road Circle"));
        });

        diceButton.onClick.AddListener(() =>
        {
            int dice1 = Random.Range(1, 7);
            int dice2 = Random.Range(1, 7);

            var resourcesDict = gameGrid.GetComponent<GameGrid>().resourcesDict;

            if (resourcesDict.ContainsKey((dice1 + dice2).ToString())) {
                print((dice1 + dice2).ToString() + "You got: " + String.Join(",", resourcesDict[(dice1 + dice2).ToString()].ToArray()));
            }
            else {
                print((dice1 + dice2).ToString() + "You got nothing lamo!");
            }
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
