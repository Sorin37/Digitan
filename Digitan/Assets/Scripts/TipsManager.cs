using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TipsManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tipsLabel;
    private float HexSize = 5;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateTip()
    {
        List<(int tipValue, string tipMessage)> tips = new List<(int tipValue, string tipMessage)>();

        int tipValue = 0;
        string tipMessage = "";

        BestSettlementSpot(out tipValue, out tipMessage);

        tips.Add((tipValue, tipMessage));

        MonopolySettlementSpot(out tipValue, out tipMessage);

        tips.Add((tipValue, tipMessage));

        EarlyGamePlaceMoreSettlements(out tipValue, out tipMessage);

        tips.Add((tipValue, tipMessage));

        MiddleGameFocus(out tipValue, out tipMessage);

        tips.Add((tipValue, tipMessage));

        string finalMessage = tips.Where(t => t.tipValue > 0).OrderBy(t => t.tipValue).FirstOrDefault().tipMessage;

        tipsLabel.text = finalMessage;
    }

    public void BestSettlementSpot(out int tipValue, out string tipMessage)
    {
        List<int> spacesPerRow = new List<int> { 3, 4, 4, 5, 5, 6, 6, 5, 5, 4, 4, 3 };

        int max = 0;
        string maxNumbers = "";

        for (int x = 0; x < spacesPerRow.Count; x++)
        {
            for (int y = 0; y < spacesPerRow[x]; y++)
            {
                int value = 0;
                string numbers = "";

                if (x < spacesPerRow.Count / 2)
                {
                    value = CollidersValue(new Vector3(
                        y * HexSize - x * HexSize / 2 + Mathf.Floor(x / 2) * HexSize / 2,
                        0.5f,
                        -Mathf.Floor(x / 2) * HexSize * 3 / 4 + HexSize / 2 - (x % 2) * HexSize / 4),
                        out numbers
                    );
                }
                else
                {
                    value = CollidersValue(new Vector3(
                           y * HexSize + (x - spacesPerRow.Count / 2) * HexSize / 2 - Mathf.Floor(x / 2) * HexSize / 2,
                           0.5f,
                           -Mathf.Floor(x / 2) * HexSize * 3 / 4 + HexSize / 2 - (x % 2) * HexSize / 4),
                           out numbers
                    );
                }

                if (max < value)
                {
                    max = value;
                    maxNumbers = numbers;
                }
            }
        }

        tipMessage = "The settlement place surrounded by the numbers: " + maxNumbers + ", gives a high probability of obtaining resources. Consider placing your settlement there!";

        tipValue = max > 9 ? 3 : 1;

        if (!Resources.FindObjectsOfTypeAll<SettlementGrid>()[0].isStartPhase)
        {
            tipValue = 1;
        }
    }

    private int CollidersValue(Vector3 pos, out string numbers)
    {
        numbers = "";

        var nearbySettlements = Physics.OverlapSphere(
            pos,
            2.5f,
            (int)
            (Mathf.Pow(2, LayerMask.NameToLayer("Settlement")) +
            Mathf.Pow(2, LayerMask.NameToLayer("My Settlement")) +
            Mathf.Pow(2, LayerMask.NameToLayer("City")))
        );

        if (nearbySettlements.Length > 0)
            return 0;

        int value = 0;

        var colliders = Physics.OverlapSphere(
            pos,
            2.5f,
            (int)Mathf.Pow(2, LayerMask.NameToLayer("Number"))
        );

        foreach (var collider in colliders)
        {
            value += collider.GetComponent<Number>().frequency;

            if (numbers.Length > 0)
                numbers += " ";
            numbers += collider.gameObject.name;
        }

        return value;
    }

    public void MonopolySettlementSpot(out int tipValue, out string tipMessage)
    {
        List<int> spacesPerRow = new List<int> { 3, 4, 4, 5, 5, 6, 6, 5, 5, 4, 4, 3 };


        for (int x = 0; x < spacesPerRow.Count; x++)
        {
            for (int y = 0; y < spacesPerRow[x]; y++)
            {
                bool foundMonopoly = false;
                string numbers = "";
                string resource = "";


                if (x < spacesPerRow.Count / 2)
                {
                    foundMonopoly = MonopolyCollidersCheck(new Vector3(
                        y * HexSize - x * HexSize / 2 + Mathf.Floor(x / 2) * HexSize / 2,
                        0.5f,
                        -Mathf.Floor(x / 2) * HexSize * 3 / 4 + HexSize / 2 - (x % 2) * HexSize / 4),
                        out numbers,
                        out resource
                    );
                }
                else
                {
                    foundMonopoly = MonopolyCollidersCheck(new Vector3(
                           y * HexSize + (x - spacesPerRow.Count / 2) * HexSize / 2 - Mathf.Floor(x / 2) * HexSize / 2,
                           0.5f,
                           -Mathf.Floor(x / 2) * HexSize * 3 / 4 + HexSize / 2 - (x % 2) * HexSize / 4),
                           out numbers,
                           out resource
                    );
                }

                if (foundMonopoly)
                {
                    tipMessage = "The settlement place surrounded by the numbers: "
                        + numbers +
                        ", will make you establish a monopoly on the "
                        + resource +
                        ". Other players will be forced to offer you advantageous trades. Consider placing your settlement there!\nYou may want to take control of the "
                        + resource + " port, in order to make the most out of your monopoly.";

                    tipValue = 2;

                    if (!Resources.FindObjectsOfTypeAll<SettlementGrid>()[0].isStartPhase)
                    {
                        tipValue = 0;
                    }

                    return;
                }
            }
        }

        tipValue = 0;
        tipMessage = "";
    }

    private bool MonopolyCollidersCheck(Vector3 pos, out string numbers, out string resource)
    {
        numbers = "";
        resource = "";

        var nearbySettlements = Physics.OverlapSphere(
            pos,
            2.5f,
            (int)
            (Mathf.Pow(2, LayerMask.NameToLayer("Settlement")) +
            Mathf.Pow(2, LayerMask.NameToLayer("My Settlement")) +
            Mathf.Pow(2, LayerMask.NameToLayer("City")))
        );

        if (nearbySettlements.Length > 0)
            return false;

        var colliders = Physics.OverlapSphere(
            pos,
            2.5f,
            (int)Mathf.Pow(2, LayerMask.NameToLayer("Number"))
        );

        if (colliders.Length != 3)
        {
            return false;
        }

        foreach (var collider in colliders)
        {
            if (collider.GetComponent<Number>().resource != colliders[0].GetComponent<Number>().resource)
                return false;

            if (numbers.Length != 0)
                numbers += " ";

            numbers += collider.GetComponent<Number>().name;
        }

        resource = colliders[0].GetComponent<Number>().resource;

        return true;
    }

    private void EarlyGamePlaceMoreSettlements(out int tipValue, out string tipMessage)
    {
        tipValue = 0;
        tipMessage = "";

        if (GetMyPlayer().nrOfVictoryPoints.Value > 5) return;

        tipValue = 2;
        tipMessage = "Extend your resource collecting pool by placing more settlements or upgrading them into cities!";
    }

    private void MiddleGameFocus(out int tipValue, out string tipMessage)
    {
        var myPlayer = GetMyPlayer();

        tipValue = 0;
        tipMessage = "";

        if (myPlayer.nrOfVictoryPoints.Value < 5) return;

        Dictionary<string, int> resourceFrequency = new Dictionary<string, int>();

        foreach (var number in myPlayer.resourcesDict.Keys)
        {
            foreach (var resource in myPlayer.resourcesDict[number])
            {
                if (resourceFrequency.ContainsKey(resource))
                {
                    resourceFrequency[resource] += NumberToFrequency(number);
                }
                else
                {
                    resourceFrequency[resource] = NumberToFrequency(number);
                }
            }
        }

        List<string> firstPlace = new List<string>();
        List<string> secondPlace = new List<string>();
        List<string> thirdPlace = new List<string>();

        List<(string resource, int frequency)> frequencies = new List<(string resource, int frequency)>();

        foreach(var key in resourceFrequency.Keys)
        {
            frequencies.Add((key, resourceFrequency[key]));
        }

        //most frequent resources
        int max = frequencies.Max(f => f.frequency);

        firstPlace.AddRange(frequencies.Where(f => f.frequency == max).Select(f => f.resource).ToList());

        //second most frequent resources
        frequencies.RemoveAll(f=> f.frequency == max);

        max = frequencies.Max(f => f.frequency);

        secondPlace.AddRange(frequencies.Where(f => f.frequency == max).Select(f => f.resource).ToList());

        //third most frequent resources
        frequencies.RemoveAll(f => f.frequency == max);

        max = frequencies.Max(f => f.frequency);

        thirdPlace.AddRange(frequencies.Where(f => f.frequency == max).Select(f => f.resource).ToList());

        List<string> top2Resources = new List<string>();
        top2Resources.AddRange(firstPlace);
        top2Resources.AddRange(secondPlace);

        if (top2Resources.Contains("Brick Resource") && top2Resources.Contains("Wood Resource"))
        {
            tipValue = 4;
            tipMessage = "Your key resources are bricks and woods. Focus on placing settlements and building roads so that you can obtain The Longest Road. Try to reach for the Brick or Wood ports as well!";
            return;
        }

        if (top2Resources.Contains("Ore Resource") && top2Resources.Contains("Grain Resource"))
        {
            tipValue = 4;
            tipMessage = "Your key resources are grains and ores. Focus on developing your settlements into cities. Try to reach for the Grain or Ore ports as well!";
            return;
        }

        List<string> top3Resources = new List<string>();
        top3Resources.AddRange(top2Resources);
        top3Resources.AddRange(thirdPlace);


        if (top3Resources.Contains("Ore Resource") && 
            top3Resources.Contains("Grain Resource") && 
            top3Resources.Contains("Wool Resource"))
        {
            tipValue = 4;
            tipMessage = "Your key resources are grain, ore and wool. Focus on acquiring developments so that you can obtain many Victory Points and The Largest Army.";
            return;
        }

        if (firstPlace.Contains("Wool Resource"))
        {
            tipValue = 3;
            tipMessage = "Your key resource is wool. Focus on reaching the wool port as fast as possible, otherwise you will fall behind! The other players might have a difficult time obtaining wool, so force them into offering you multiple resources for wool when trading.";
            return;
        }
    }

    private Player GetMyPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().IsOwner)
                return p.GetComponent<Player>();
        }

        return null;
    }

    private int NumberToFrequency(string number)
    {
        switch (number)
        {
            case "2": return 1;
            case "3": return 2;
            case "4": return 3;
            case "5": return 4;
            case "6": return 5;
            case "8": return 5;
            case "9": return 4;
            case "10": return 3;
            case "11": return 2;
            case "12": return 1;
            default: return 0;
        }
    }
}
