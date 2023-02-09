using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClientDetails : MonoBehaviour
{
    [SerializeField] private Button connectButton;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField ipInput;

    private void Awake()
    {
        connectButton.onClick.AddListener(() =>
        {
            print(nameInput.text);
            print(ipInput.text);

            SceneManager.LoadScene("Lobby");
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
