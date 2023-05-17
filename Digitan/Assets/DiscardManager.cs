using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiscardManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI BrickLabel;
    [SerializeField] private TextMeshProUGUI GrainLabel;
    [SerializeField] private TextMeshProUGUI LumberLabel;
    [SerializeField] private TextMeshProUGUI OreLabel;
    [SerializeField] private TextMeshProUGUI WoolLabel;
    [SerializeField] private Button SubtractBrick;
    [SerializeField] private Button AddBrick;
    [SerializeField] private Button SubtractGrain;
    [SerializeField] private Button AddGrain;
    [SerializeField] private Button SubtractLumber;
    [SerializeField] private Button AddLumber;
    [SerializeField] private Button SubtractOre;
    [SerializeField] private Button AddOre;
    [SerializeField] private Button SubtractWool;
    [SerializeField] private Button AddWool;
    [SerializeField] private Button ConfirmButton;

    public Dictionary<string, int> discardDict = new Dictionary<string, int>();
    public Dictionary<string, int> hand = new Dictionary<string, int>();


    // Start is called before the first frame update
    void Start()
    {
        InitDiscardDict();
        InitDiscardButtons();
        InitConfirmButton();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void InitDiscardDict()
    {
        hand = GetMyPlayer().GetComponent<Player>().playerHand;

        foreach (var key in hand.Keys)
        {
            discardDict[key] = 0;
        }
    }

    private void Add(string resource)
    {
        var key = resource + " Resource";

        if (hand[key] > discardDict[key])
        {
            discardDict[key]++;
        }
    }

    private void Subtract(string resource)
    {
        var key = resource + " Resource";

        if (discardDict[key] > 0)
        {
            discardDict[key]--;
        }
    }
    private void InitDiscardButtons()
    {
        //add
        AddBrick.onClick.AddListener(() =>
        {
            Add("Brick");
            DrawDiscardDict();
        });

        AddGrain.onClick.AddListener(() =>
        {
            Add("Grain");
            DrawDiscardDict();
        });

        AddLumber.onClick.AddListener(() =>
        {
            Add("Lumber");
            DrawDiscardDict();
        });

        AddOre.onClick.AddListener(() =>
        {
            Add("Ore");
            DrawDiscardDict();
        });

        AddWool.onClick.AddListener(() =>
        {
            Add("Wool");
            DrawDiscardDict();
        });

        //subtract
        SubtractBrick.onClick.AddListener(() =>
        {
            Subtract("Brick");
            DrawDiscardDict();
        });

        SubtractGrain.onClick.AddListener(() =>
        {
            Subtract("Grain");
            DrawDiscardDict();
        });

        SubtractLumber.onClick.AddListener(() =>
        {
            Subtract("Lumber");
            DrawDiscardDict();
        });

        SubtractOre.onClick.AddListener(() =>
        {
            Subtract("Ore");
            DrawDiscardDict();
        });

        SubtractWool.onClick.AddListener(() =>
        {
            Subtract("Wool");
            DrawDiscardDict();
        });
    }

    public void DrawDiscardDict()
    {
        BrickLabel.text = "x " + discardDict["Brick Resource"].ToString();
        GrainLabel.text = "x " + discardDict["Grain Resource"].ToString();
        LumberLabel.text = "x " + discardDict["Lumber Resource"].ToString();
        OreLabel.text = "x " + discardDict["Ore Resource"].ToString();
        WoolLabel.text = "x " + discardDict["Wool Resource"].ToString();
    }

    private void InitConfirmButton()
    {
        ConfirmButton.onClick.AddListener(() =>
        {
            int handSum = 0;
            int discardSum = 0;

            foreach(var entry in hand)
            {
                handSum += entry.Value;
            }

            foreach(var entry in discardDict)
            {
                discardSum += entry.Value;
            }

            if (handSum / 2 == discardSum)
            {
                foreach(var key in discardDict.Keys)
                {
                    hand[key] -= discardDict[key];
                }

                transform.parent.gameObject.SetActive(false);
                GetMyPlayer().GetComponent<Player>().UpdateHand();
            }
        });
    }

    private GameObject GetMyPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().IsOwner)
                return p;
        }

        return null;
    }
}
