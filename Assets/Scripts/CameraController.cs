using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform player;             // Assign your player Transform here

    [Header("Distance & Offset")]
    public float distance = 5f;          // Max orbit radius
    public Vector3 offset = Vector3.zero;// Additional pivot offset

    [Header("Collision")]
    public float collisionRadius = 0.3f;     // Radius of sphere‐cast
    public LayerMask obstructionMask;        // Which layers to treat as solid

    [Header("Sensitivity")]
    public float mouseSensitivityX = 100f;
    public float mouseSensitivityY = 100f;

    [Header("Pitch Limits")]
    public float minPitch = -20f;
    public float maxPitch =  80f;

    private float yaw;   // horizontal angle around player
    private float pitch; // vertical angle

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
        Vector3 angles   = transform.eulerAngles;
        yaw   = angles.y;
        pitch = angles.x;
    }

    void Update()
    {
        // Toggle cursor lock
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool locked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible   = locked;
        }

        // Mouse look
        float deltaX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        float deltaY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
        yaw   += deltaX;
        pitch  = Mathf.Clamp(pitch - deltaY, minPitch, maxPitch);

        // Build full-3D orbit rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Desired camera offset (before collision)
        Vector3 desiredOffset = rotation * new Vector3(0f, 0f, -distance) + offset;
        Vector3 origin        = player.position + offset;

        // Sphere-cast toward desired position
        RaycastHit hit;
        float trueDist = distance;
        Vector3 rayDir = (desiredOffset).normalized;
        if (Physics.SphereCast(origin, collisionRadius, rayDir, out hit, distance, obstructionMask))
        {
            trueDist = hit.distance;
        }

        // Apply final position & rotation
        Vector3 finalOffset = rotation * new Vector3(0f, 0f, -trueDist) + offset;
        transform.position  = player.position + finalOffset;
        transform.rotation  = rotation;
    }
}