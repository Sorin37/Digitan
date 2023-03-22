using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StartGame : NetworkBehaviour
{
    [SerializeField] private GameObject gameGrid;
    private GameObject xd;

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
        xd = Instantiate(gameGrid);
        xd.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc]
    public void SecondServerRpc()
    {
        xd.GetComponent<GameGrid>().CreateGrid();
    }

}
