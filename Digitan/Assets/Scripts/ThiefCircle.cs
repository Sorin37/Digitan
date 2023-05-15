using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThiefCircle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseDown()
    {
        //necessary piece of code in order to prevent clicking through UI elements
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Thief Circle"));

        GetHostPlayer().GetComponent<Player>().MoveThiefServerRpc(transform.position);

        var colliders = Physics.OverlapSphere(
            transform.position,
            50f,
            (int)(Mathf.Pow(2, LayerMask.NameToLayer("City")) + 
            Mathf.Pow(2, LayerMask.NameToLayer("Settlement")))
        );


        foreach (var collider in colliders)
        {
            print(collider.gameObject.name);
        }

        print("Number of nearby cities of settlements" + colliders.Length);

        //DisplayStealBoard();
    }

    private GameObject GetHostPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().IsOwnedByServer)
                return p;
        }

        return null;
    }

    private void DisplayStealBoard()
    {
        var stealManager = Resources.FindObjectsOfTypeAll<StealManager>()[0];
        stealManager.gameObject.transform.parent.gameObject.SetActive(true);
    }
}
