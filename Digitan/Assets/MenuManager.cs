using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button connectButton;

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Game");
        });

        hostButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Host");
        });

        connectButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Client");
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
