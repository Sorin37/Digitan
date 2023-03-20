using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StartGame : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            FirstStepTowardsSuccessClientRpc();
        }
    }

    [ClientRpc]
    private void FirstStepTowardsSuccessClientRpc()
    {
        print("SUCCES LUME");
    }
}
