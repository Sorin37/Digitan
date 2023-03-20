using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StartGame : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            print("xd?");
            FirstStepTowardsSuccessClientRpc();
        }
    }



    // Update is called once per frame
    void Update()
    {

    }

    [ClientRpc]
    private void FirstStepTowardsSuccessClientRpc()
    {
        print("SUCCES LUME");
    }
}
