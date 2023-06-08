using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupHostManager : MonoBehaviour
{
    [SerializeField] private Button okButton;
    [SerializeField] private GameObject popupCanvas;
    [SerializeField] private TextMeshProUGUI message;

    private void Awake()
    {
        okButton.onClick.AddListener(() =>
        {
            popupCanvas.gameObject.SetActive(false);
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

    public void SetErrorMessage(string message)
    {
        this.message.text = message;
    }
}
