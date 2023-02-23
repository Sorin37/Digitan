using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayersList : MonoBehaviour
{
    [SerializeField] private GameObject PlayerDetails;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject playerDetails = Instantiate(
                PlayerDetails, 
                new Vector3(0, i * 150 - 225, 0), 
                transform.rotation
            );

            playerDetails.transform.SetParent(transform, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
