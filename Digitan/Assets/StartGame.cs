using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StartGame : NetworkBehaviour
{
    [SerializeField] private Transform gameGrid;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }



    // Update is called once per frame
    void Update()
    {
        if(!IsOwner) return;
        if (Input.GetKeyUp(KeyCode.W))
        {
            FirstStepTowardsSuccessServerRpc();
        }
    }

    [ServerRpc]
    public void FirstStepTowardsSuccessServerRpc()
    {
        print("SUCCES LUME");
        Transform _gameGrid = Instantiate(gameGrid);

    }
}
