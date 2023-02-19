using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    [SerializeField] private Button okButton;
    [SerializeField] private Canvas popupCanvas;
    [SerializeField] private Canvas inputCanvas;

    private void Awake()
    {
        okButton.onClick.AddListener(() =>
        {
            popupCanvas.gameObject.SetActive(false);
            inputCanvas.gameObject.SetActive(true);
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
