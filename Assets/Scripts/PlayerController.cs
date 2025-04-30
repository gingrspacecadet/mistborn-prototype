using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTarget;      // Where we read forward/right from
    public Transform groundCheck;       // Empty at your feet

    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce = 5f;

    [Header("Ground Check")]
    public float groundDistance = 0.2f; // radius of our sphere check
    public LayerMask groundMask;        // which layers count as “ground”

    private Rigidbody rb;
    private float currentSpeed;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        // 1) Ground-check
        isGrounded = Physics.CheckSphere(
            groundCheck.position, 
            groundDistance, 
            groundMask
        );

        // 2) Speed toggle
        currentSpeed = Input.GetKey(KeyCode.LeftShift)
            ? sprintSpeed
            : walkSpeed;

        // 3) Build desired velocity
        Vector3 forward = cameraTarget.forward;
        Vector3 right   = cameraTarget.right;
        forward.y = right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 dir = Vector3.zero;
        if      (Input.GetKey(KeyCode.W)) dir =  forward;
        else if (Input.GetKey(KeyCode.S)) dir = -forward;
        else if (Input.GetKey(KeyCode.D)) dir =  right;
        else if (Input.GetKey(KeyCode.A)) dir = -right;

        Vector3 targetVel = dir * currentSpeed;
        targetVel.y = rb.linearVelocity.y; // preserve vertical

        rb.linearVelocity = targetVel;

        // 4) Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }
    }

    // (Optional) visualize the ground-check sphere in the scene view
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}