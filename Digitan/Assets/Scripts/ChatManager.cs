using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private GameObject scroll;
    [SerializeField] private GameObject content;

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
            Canvas.ForceUpdateCanvases();

            content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
            content.GetComponent<ContentSizeFitter>().SetLayoutVertical();

            scroll.GetComponent<ScrollRect>().content.GetComponent<VerticalLayoutGroup>().CalculateLayoutInputVertical();
            scroll.GetComponent<ScrollRect>().content.GetComponent<ContentSizeFitter>().SetLayoutVertical();

            scroll.GetComponent<ScrollRect>().normalizedPosition = new Vector2(0, 0);
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
