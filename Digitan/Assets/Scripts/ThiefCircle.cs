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

        Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("Thief Circle"));

        GetHostPlayer().GetComponent<Player>().MoveThiefServerRpc(transform.position);
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
}
