using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private TMP_InputField chatInput;
    private float cameraVerticalRotation = 82f;
    private float cameraHorizontalRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!chatInput.isFocused)
        {
            HandleCameraMovement();
        }
    }

    private void HandleCameraMovement()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            cameraVerticalRotation -= 0.1f;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.S))
        {
            cameraVerticalRotation += 0.1f;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.A))
        {
            cameraHorizontalRotation -= 0.1f;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D))
        {
            cameraHorizontalRotation += 0.1f;
        }
        else if (Input.GetKey(KeyCode.W)) { inputDir.z = +1f; }
        else if (Input.GetKey(KeyCode.S)) { inputDir.z = -1f; }
        else if (Input.GetKey(KeyCode.A)) { inputDir.x = -1f; }
        else if (Input.GetKey(KeyCode.D)) { inputDir.x = +1f; }
        else if (Input.GetKey(KeyCode.U)) { inputDir.y = +1f; }
        else if (Input.GetKey(KeyCode.J))
        {
            if (gameObject.transform.position.y > 1f)
                inputDir.y = -1f;
        }
        else if (Input.GetKey(KeyCode.R))
        {
            gameObject.transform.position = new Vector3(4, 22, -11);
            gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
            cameraVerticalRotation = 82f;
            cameraHorizontalRotation = 0f;
        }

        //rotate up down, left right
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);

        gameObject.transform.eulerAngles = Vector3.right * cameraVerticalRotation + Vector3.up * cameraHorizontalRotation;

        //movement
        Vector3 moveDir = gameObject.transform.forward * inputDir.z + gameObject.transform.right * inputDir.x;

        float moveSpeed = 10f;
        gameObject.transform.position += inputDir * moveSpeed * Time.deltaTime;
    }
}
