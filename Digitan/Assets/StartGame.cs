using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StartGame : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print(NetworkManager.Singleton.IsHost);
        print(NetworkManager.Singleton.IsServer);
        print(NetworkManager.Singleton.IsClient);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
