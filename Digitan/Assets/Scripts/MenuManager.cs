using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button connectButton;
    [SerializeField] private Button copyrightButton;
    [SerializeField] private GameObject copyrightCanvas;

    private void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Host");
        });

        connectButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Client");
        });

        copyrightButton.onClick.AddListener(() =>
        {
            copyrightCanvas.SetActive(true);
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
