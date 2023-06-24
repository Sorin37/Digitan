using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.ScrollRect;

public class DiceManager : MonoBehaviour
{
    [SerializeField] private Image redDice;
    [SerializeField] private Image yellowDice;
    private ButtonManager buttonManager;
    private Vector3 pulseTarget = new Vector3(1.2f, 1.2f, 1);
    private bool isRising = true;
    public bool shouldPulsate = false;

    void Awake()
    {
        buttonManager = Resources.FindObjectsOfTypeAll<ButtonManager>()[0];
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        AnimateDices();
    }

    private void AnimateDices()
    {
        if (!shouldPulsate)
        {
            return;
        }

        if (!buttonManager.hasRolledDice)
        {
            PulsateDices();
        }
        else
        {
            ResetDices();
        }
    }

    private void PulsateDices()
    {
        if (isRising)
        {
            redDice.transform.localScale = Vector3.Lerp(redDice.transform.localScale, pulseTarget, Time.deltaTime * 5);
            yellowDice.transform.localScale = Vector3.Lerp(yellowDice.transform.localScale, pulseTarget, Time.deltaTime * 5);
            if (redDice.transform.localScale.x > pulseTarget.x - 0.01f)
            {
                isRising = false;
            }
        }
        else
        {
            redDice.transform.localScale = Vector3.Lerp(redDice.transform.localScale, Vector3.one, Time.deltaTime * 5);
            yellowDice.transform.localScale = Vector3.Lerp(yellowDice.transform.localScale, Vector3.one, Time.deltaTime * 5);
            if (redDice.transform.localScale.x < 1f + 0.01f)
            {
                isRising = true;
            }
        }

    }

    public void ResetDices()
    {
        redDice.transform.localScale = Vector3.one;
        yellowDice.transform.localScale = Vector3.one;
    }
}
