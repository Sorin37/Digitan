using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private Slider loadingBar;
    private float checkStartTimer = 2;
    private float totalAmountOfTime = 0;
    private bool displayedError = false;

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

            if (totalAmountOfTime > 90 && !displayedError)
            {
                displayedError = true;

                var failedToConnect = Resources.FindObjectsOfTypeAll<FailedToConnectManager>()[0];

                failedToConnect.SetErrorMessage("One of the players failed to connect. Please try again!");
                failedToConnect.gameObject.SetActive(true);

                gameObject.SetActive(false);
            }
        }
    }
}
