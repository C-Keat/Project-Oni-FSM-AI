using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{

    // stored names for the inputs of mouse x and mouse y
    [SerializeField] private string mouseXInputName, mouseYInputName;
    // storing the value of the mouse sensitivity 
    [SerializeField] private float mouseSensitivity;
    // refrence to the player body transform
    [SerializeField] private Transform playerBody;
    //variable that will stop the camera from moving.
    private float xAxisClamp;

    // things that need to happen at the start of the level
    private void Awake()
    {
        LockCursor();
        // value insitilaised 
        xAxisClamp = 0.0f;
    }


    // function locking the cursor to the center of the screen
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CameraRotation();
    }

    // getting the value of the inputs of the mouse.
    private void CameraRotation()
    {

    // allowing the mouse sensitivity to effect the mouse movement
        float mouseX = Input.GetAxis(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        // locking the camera when it moves to far up or too low
        xAxisClamp += mouseY;
        if (xAxisClamp > 60.0f)
        {
            xAxisClamp = 60.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(240.0f);
        }
        else if (xAxisClamp < -60.0f)
        {
            xAxisClamp = -60.0f;
            mouseY = 0.0f;
            ClampXAxisRotationToValue(60.0f);
        }

        // rotating the camera 
        transform.Rotate(Vector3.left * mouseY);
        // rotating the body
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void ClampXAxisRotationToValue(float value)
    {
        // setting the x axis value to to the camera rotation.
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }
}
 

