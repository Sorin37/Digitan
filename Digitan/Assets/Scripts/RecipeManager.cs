using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject recipeCanvas;

    // Start is called before the first frame update
    void Start()
    {
        InitCloseButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void InitCloseButton()
    {
        closeButton.onClick.AddListener(() =>
        {
            recipeCanvas.SetActive(false);
        });
    }
}
