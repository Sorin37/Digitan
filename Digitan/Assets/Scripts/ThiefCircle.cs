using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies.Models;
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

        GetMyPlayer().hasToMoveThief = false;

        var hostPlayer = GetHostPlayer().GetComponent<Player>();

        string number = null;
        string resource = null;

        var thief = GameObject.Find("Thief");

        if (thief != null)
        {
            var freeResource = Physics.OverlapSphere(
                thief.transform.position,
                1.5f,
                (int)Mathf.Pow(2, LayerMask.NameToLayer("Number"))
            );

            if (freeResource.Length > 0)
            {
                number = freeResource[0].gameObject.name;
                resource = freeResource[0].GetComponent<Number>().resource;
            }
        }

        FreeResource(hostPlayer, number, resource);

        number = null;
        resource = null;

        //move the thief
        hostPlayer.MoveThiefServerRpc(transform.position);

        //block the new resource
        var numberObject = Physics.OverlapSphere(
            transform.position,
            1.5f,
            (int)Mathf.Pow(2, LayerMask.NameToLayer("Number"))
        );

        if (numberObject.Count() == 0)
        {
            number = null;
            resource = null;
        }
        else
        {
            number = numberObject[0].gameObject.name;
            resource = numberObject[0].GetComponent<Number>().resource;
        }

        BlockResource(hostPlayer, number, resource);

        var colliders = Physics.OverlapSphere(
            transform.position,
            3f,
            (int)(Mathf.Pow(2, LayerMask.NameToLayer("City")) +
            Mathf.Pow(2, LayerMask.NameToLayer("Settlement")))
        );

        //get the id of the victims in order to decide who steal from
        HashSet<ulong> ids = new HashSet<ulong>();

        foreach (var collider in colliders)
        {
            var settlementPiece = collider.GetComponent<SettlementPiece>();
            if (settlementPiece != null)
            {
                ids.Add(settlementPiece.playerId);
                continue;
            }

            var cityPiece = collider.GetComponent<CityPiece>();
            if (cityPiece != null)
            {
                ids.Add(cityPiece.playerId);
            }
        }

        //form the playerDetails list
        List<StealManager.PlayerDetails> players = new List<StealManager.PlayerDetails>();

        foreach (var id in ids)
        {
            var player = GetPlayerWithId(id).GetComponent<Player>();

            players.Add(new StealManager.PlayerDetails
            {
                id = id,
                name = player.nickName.Value.ToString(),
                color = player.IdToColor(id)
            });
        }

        if (players.Count > 1)
        {
            DisplayStealBoard(players);
        }

        if (players.Count == 1)
        {
            GetHostPlayer().GetComponent<Player>().StealServerRpc(players[0].id, new Unity.Netcode.ServerRpcParams());
        }
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

    private void DisplayStealBoard(List<StealManager.PlayerDetails> players)
    {
        var stealManager = Resources.FindObjectsOfTypeAll<StealManager>()[0];

        stealManager.DisplayPlayersToStealFrom(players);
        stealManager.gameObject.transform.parent.gameObject.SetActive(true);
    }

    private GameObject GetPlayerWithId(ulong id)
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().OwnerClientId == id)
                return p;
        }

        return null;
    }

    private void FreeResource(Player hostPlayer, string number, string resource)
    {
        if (number == null)
            return;

        var thief = GameObject.Find("Thief");

        if (thief == null)
        {
            return;
        }

        var settlements = Physics.OverlapSphere(
            thief.transform.position,
            3f,
            (int)(Mathf.Pow(2, LayerMask.NameToLayer("My Settlement")) +
            Mathf.Pow(2, LayerMask.NameToLayer("Settlement")))
        );

        foreach (var settlement in settlements)
        {
            hostPlayer.ModifyResourceDictServerRpc("Free", number, resource, settlement.GetComponent<SettlementPiece>().playerId);
        }

        var cities = Physics.OverlapSphere(
            thief.transform.position,
            3f,
            (int)Mathf.Pow(2, LayerMask.NameToLayer("City"))
        );

        foreach (var city in cities)
        {
            hostPlayer.ModifyResourceDictServerRpc("Free", number, resource, city.GetComponent<CityPiece>().playerId);
            hostPlayer.ModifyResourceDictServerRpc("Free", number, resource, city.GetComponent<CityPiece>().playerId);
        }
    }

    private void BlockResource(Player hostPlayer, string number, string resource)
    {
        if (number == null)
            return;

        var settlements = Physics.OverlapSphere(
            transform.position,
            3f,
            (int)(Mathf.Pow(2, LayerMask.NameToLayer("My Settlement")) +
            Mathf.Pow(2, LayerMask.NameToLayer("Settlement")))
        );

        foreach (var settlement in settlements)
        {
            hostPlayer.ModifyResourceDictServerRpc("Block", number, resource, settlement.GetComponent<SettlementPiece>().playerId);
        }

        var cities = Physics.OverlapSphere(
            transform.position,
            3f,
            (int)Mathf.Pow(2, LayerMask.NameToLayer("City"))
        );

        foreach (var city in cities)
        {
            hostPlayer.ModifyResourceDictServerRpc("Block", number, resource, city.GetComponent<CityPiece>().playerId);
            hostPlayer.ModifyResourceDictServerRpc("Block", number, resource, city.GetComponent<CityPiece>().playerId);
        }
    }

    private Player GetMyPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().IsOwner)
                return p.GetComponent<Player>();
        }

        return null;
    }
}
