using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private Slider loadingBar;
    [SerializeField] private GameObject failedToConnectCanvas;
    private float checkStartTimer = 2;
    private float totalAmountOfTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayersFailedToConnect();
    }

    public void SetProgress(int currentValue, int maxValue)
    {
        loadingBar.value = (float)currentValue / maxValue;
    }

    private void CheckPlayersFailedToConnect()
    {
        checkStartTimer -= Time.deltaTime;

        if (checkStartTimer < 0f)
        {
            float checkStartTimerMax = 2;

            checkStartTimer = checkStartTimerMax;

            totalAmountOfTime += checkStartTimer;

            if (totalAmountOfTime > 90)
            {
                failedToConnectCanvas.GetComponent<FailedToConnectManager>()
                    .SetErrorMessage("One of the players failed to connect. Please try again!");
                failedToConnectCanvas.SetActive(true);

                gameObject.SetActive(false);
            }
        }
    }
}
