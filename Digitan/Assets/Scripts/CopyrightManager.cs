using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyrightManager : MonoBehaviour
{
    [SerializeField] private Button closeButton;

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
            gameObject.SetActive(false);
        });
    }
}
