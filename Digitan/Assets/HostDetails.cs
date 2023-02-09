using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class HostDetails : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private TMP_InputField nameInput;

    private void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
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
