using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.EventSystems;

public class CityPlace : MonoBehaviour
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

        AddToResourcesDict();

        Destroy(gameObject);

        GetHostPlayer().GetComponent<Player>().PlaceCityServerRpc(
            transform.position.x,
            transform.position.y,
            transform.position.z,
            GetMyPlayer().GetComponent<Player>().color,
            new Unity.Netcode.ServerRpcParams()
        );

        Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << LayerMask.NameToLayer("City Place"));
        Camera.main.cullingMask = Camera.main.cullingMask | (1 << LayerMask.NameToLayer("My Settlement"));

        GetMyPlayer().GetComponent<Player>().nrOfPlacedCities++;
        GetMyPlayer().GetComponent<Player>().nrOfPlacedSettlement--;
    }

    private void AddToResourcesDict()
    {
        var resourcesDict = GetMyPlayer().GetComponent<Player>().resourcesDict;

        var colliders = Physics.OverlapSphere(
            transform.position,
            2.5f,
            (int)Mathf.Pow(2, LayerMask.NameToLayer("Number"))
        );

        foreach (var collider in colliders)
        {
            resourcesDict[collider.gameObject.name].Add(collider.gameObject.GetComponent<Number>().resource);
        }
    }

    private GameObject GetMyPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().IsOwner)
                return p;
        }

        return null;
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
