using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraX : MonoBehaviour
{
    public Transform orientation;

    [Header("Mouse Sensitivity")]
    [SerializeField] float mouseSensitivityX = 300f;
    [SerializeField] float mouseSensitivityY = 300f;

    private float xRotation;
    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }  

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); 

        // Rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
