using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonopolyMessageManager : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI message;

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

    public void SetMessage(string playerName, string resource)
    {
        message.text = playerName + " established a monopoly on the " + resource;
    }
}
