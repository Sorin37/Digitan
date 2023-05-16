using System.Collections;
using System.Collections.Generic;
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

        GetHostPlayer().GetComponent<Player>().MoveThiefServerRpc(transform.position);

        var colliders = Physics.OverlapSphere(
            transform.position,
            2.5f,
            (int)(Mathf.Pow(2, LayerMask.NameToLayer("City")) +
            Mathf.Pow(2, LayerMask.NameToLayer("Settlement")))
        );

        //get the id of the victims
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
}
