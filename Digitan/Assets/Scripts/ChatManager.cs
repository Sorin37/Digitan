using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField chatInput;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            GetHostPlayer().SendChatMessageServerRpc(chatInput.text, new Unity.Netcode.ServerRpcParams());
            chatInput.text = string.Empty;
            chatInput.Select();
            chatInput.ActivateInputField();
        }
    }

    private Player GetHostPlayer()
    {
        var players = GameObject.FindGameObjectsWithTag("Player");

        foreach (var p in players)
        {
            if (p.GetComponent<Player>().IsOwnedByServer)
                return p.GetComponent<Player>();
        }

        return null;
    }
}
