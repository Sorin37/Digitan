using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryManager : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI message;

    // Start is called before the first frame update
    void Start()
    {
        InitExitButton();
    }

    private void InitExitButton()
    {
        exitButton.onClick.AddListener(() =>
        {
            var go = new GameObject("Sacrificial Lamb");
            DontDestroyOnLoad(go);

            foreach (var root in go.scene.GetRootGameObjects())
                Destroy(root);

            SceneManager.LoadScene("MainMenu");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMessage(string winnerName)
    {
        message.text = winnerName + " won the game!";
    }
}
