using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

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
        var brick = xd.GetComponent<GameGrid>().brick;
        var brickGO = Instantiate(
                    brick,
                    transform.position,
                    Quaternion.Euler(-90, 180, 0)
                    );
        brickGO.GetComponent<NetworkObject>().Spawn();
        brickGO.transform.parent = xd.transform;

    }
}
