using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscMenuManager : MonoBehaviour
{
    [SerializeField] private Button exitGameButton;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        InitExitGameButton();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var escMenuCanvas = gameObject.transform.Find("EscMenuCanvas").gameObject;
            escMenuCanvas.SetActive(!escMenuCanvas.activeSelf);
        }
    }

    private void InitExitGameButton()
    {
        exitGameButton.onClick.AddListener(() => {
            Application.Quit();
        });
    }
}
