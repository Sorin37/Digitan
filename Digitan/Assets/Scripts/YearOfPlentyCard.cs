using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class YearOfPlentyCard : MonoBehaviour
{
    [SerializeField] private Image card;
    private Color hoverColor = new Color(195f/255, 176f/255, 176f/255, 255f);
    private Color clickedColor = new Color(144f/255, 134f/255, 134f/255, 255f);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnEnterAnimation()
    {
        card.color = hoverColor;
    }

    public void OnExitAnimation()
    {
        card.color = Color.white;
    }

    public void OnMouseDownAnimation()
    {
        card.color = clickedColor;
    }

    public void OnMouseUpAnimation()
    {
        card.color = hoverColor;
    }
}
