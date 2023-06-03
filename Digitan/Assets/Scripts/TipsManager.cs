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
        List <(int tipValue, string tipMessage)> tips = new List<(int tipValue, string tipMessage)> ();

        int tipValue = 0;
        string tipMessage = "";

        BestSettlementSpot(out tipValue, out tipMessage);

        tips.Add((tipValue, tipMessage));

        string finalMessage = tips.OrderBy(t => t.tipValue).FirstOrDefault().tipMessage;

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
}
