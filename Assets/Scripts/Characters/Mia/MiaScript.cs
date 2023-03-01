using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class MiaScript : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public float mouseSensitivity = 100f;
    private Rigidbody rb;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundDistance, groundMask);

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 movement = (transform.right * horizontalInput + transform.forward * verticalInput).normalized;
        movement = movement * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.fixedDeltaTime;

        transform.Rotate(Vector3.up * mouseX);
    }
}