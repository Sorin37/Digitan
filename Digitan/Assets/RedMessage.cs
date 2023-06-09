using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class RedMessage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStartPosition(Transform caller)
    {
        var messageRectTransform = gameObject.transform as RectTransform;

        gameObject.transform.position = new Vector3(
            caller.position.x,
            caller.position.y + messageRectTransform.sizeDelta.y,
            caller.position.z
        );

        if (messageRectTransform.sizeDelta.x / 2 + gameObject.transform.position.x > 1920)
        {
            gameObject.transform.position = new Vector3(
            gameObject.transform.position.x - (messageRectTransform.sizeDelta.x / 2 + gameObject.transform.position.x - 1920),
            gameObject.transform.position.y,
            gameObject.transform.position.z
        );
        }
    }
}
