using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

public class RedMessage : MonoBehaviour
{
    private bool positionSet = false;
    private Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (positionSet)
        {
            FadeAway();
        }
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

        targetPosition = new Vector3(
            gameObject.transform.position.x,
            gameObject.transform.position.y + 360,
            gameObject.transform.position.z
        );

        positionSet = true;
    }

    private void FadeAway()
    {
        gameObject.transform.position = Vector3.MoveTowards(
            gameObject.transform.position,
            targetPosition,
            100 * Time.deltaTime
        );
        if (gameObject.transform.position == targetPosition)
            Destroy(gameObject);
    }
}
