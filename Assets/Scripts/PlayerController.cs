using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPlayerController : MonoBehaviour
{
    private Rigidbody rb;

    public GameObject cameraTarget;

    public float movementIntensity;
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = walkSpeed; // Start with walking speed
    }

    void Update()
    {
        var ForwardDirection = cameraTarget.transform.forward;
        var RightDirection = cameraTarget.transform.right;

        // Normalize directions to ensure consistent speed
        ForwardDirection.y = 0;
        RightDirection.y = 0;
        ForwardDirection.Normalize();
        RightDirection.Normalize();

        // Check for sprint input
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        // Move Forwards
        if (Input.GetKey(KeyCode.W)) 
        {
            rb.linearVelocity = ForwardDirection * currentSpeed;
        }
        // Move Backwards
        else if (Input.GetKey(KeyCode.S))
        {
            rb.linearVelocity = -ForwardDirection * currentSpeed;
        }
        // Move Rightwards
        else if (Input.GetKey(KeyCode.D))
        {
            rb.linearVelocity = RightDirection * currentSpeed;
        }
        // Move Leftwards
        else if (Input.GetKey(KeyCode.A))
        {
            rb.linearVelocity = -RightDirection * currentSpeed;
        }
        else
        {
            rb.linearVelocity = Vector3.zero; // Stop movement when no key is pressed
        }
    }
}