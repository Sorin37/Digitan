using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KickedManager : MonoBehaviour
{
    [SerializeField] private Button okButton;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject.transform.parent.gameObject);
        var go = new GameObject("Sacrificial Lamb");
        DontDestroyOnLoad(go);

        foreach (var root in go.scene.GetRootGameObjects())
        {
            if (root.name == "KickCanvas")
                continue;

            Destroy(root);
        }

        SceneManager.LoadScene("MainMenu");

        InitOkButton();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InitOkButton()
    {
        okButton.onClick.AddListener(() =>
        {
            gameObject.transform.parent.gameObject.SetActive(false);
        });
    }
}
