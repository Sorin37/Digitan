using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private Slider loadingBar;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetProgress(int currentValue, int maxValue)
    {
        loadingBar.value = (float)maxValue/currentValue;
    }
}
