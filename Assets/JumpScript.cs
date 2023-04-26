using UnityEngine;

public class JumpScript : MonoBehaviour
{

    public float jumpForce = 5000f; // The force of the jump
    public float antiGravityForce = 10f; // The force of the anti-gravity

    public Rigidbody rb; // Reference to the Rigidbody component

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Check if the spacebar is pressed
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Add the upwards force to the Rigidbody
        }
        if (!IsGrounded(rb))
        {
            rb.AddForce(Vector3.down * antiGravityForce, ForceMode.Impulse); // Add the downwards force to the Rigidbody
        }
    }

    public static bool IsGrounded(Rigidbody rb)
    {
        return Physics.Raycast(rb.transform.position, -Vector3.up, rb.transform.localScale.y * 0.6f);
    }


}
