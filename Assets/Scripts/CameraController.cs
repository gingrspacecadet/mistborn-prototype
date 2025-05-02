using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Distance & Offset")]
    public float distance = 5f;
    public Vector3 offset = Vector3.zero;

    [Header("Collision")]
    public float collisionRadius = 0.3f;
    public LayerMask obstructionMask;

    [Header("Sensitivity")]
    public float mouseSensitivityX = 100f;
    public float mouseSensitivityY = 100f;

    [Header("Pitch Limits")]
    public float minPitch = -20f;
    public float maxPitch = 80f;

    private float yaw;
    private float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool locked = Cursor.lockState == CursorLockMode.Locked;
            Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = locked;
        }

        float deltaX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        float deltaY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
        yaw += deltaX;
        pitch = Mathf.Clamp(pitch - deltaY, minPitch, maxPitch);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        Vector3 desiredOffset = rotation * new Vector3(0f, 0f, -distance) + offset;
        Vector3 origin = player.position + offset;

        RaycastHit hit;
        float trueDist = distance;
        Vector3 rayDir = (desiredOffset).normalized;
        if (Physics.SphereCast(origin, collisionRadius, rayDir, out hit, distance, obstructionMask))
        {
            trueDist = hit.distance;
        }

        Vector3 finalOffset = rotation * new Vector3(0f, 0f, -trueDist) + offset;
        transform.position = player.position + finalOffset;
        transform.rotation = rotation;
    }
}