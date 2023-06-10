using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyExceptionManager : MonoBehaviour
{
    [SerializeField] private Button okButton;
    [SerializeField] private TextMeshProUGUI message;

    // Start is called before the first frame update
    void Start()
    {
        okButton.onClick.AddListener(() =>
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

    public void SetErrorMessage(string message)
    {
        this.message.text = message;
    }
}
