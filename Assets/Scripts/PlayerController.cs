using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine.Rendering.Universal;
using System;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform    cameraTarget;   // Where we read forward/right from
    public Transform    groundCheck;    // Empty at your feet
    public Allomancer   allomancer;     // Reference to your Allomancer component
    public Material     lineMaterial;   // Assign a basic unlit transparent material in Inspector

    [Header("Movement")]
    public float walkSpeed   = 5f;
    public float sprintSpeed = 10f;
    public float jumpForce   = 5f;

    [Header("Ground Check")]
    public float   groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Iron/Steel Settings")]
    public float metalDetectRadius = 10f;
    public float pushPullForce = 50f;
    public LayerMask metalLayer;

    private List<Rigidbody> nearbyMetals = new List<Rigidbody>();
    private List<LineRenderer> activeLines = new List<LineRenderer>();

    private Rigidbody rb;
    private float currentSpeed;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    [System.Obsolete]
    void Update()
    {
        // 1) Ground-check
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );

        // 2) Base speed (walk vs sprint)
        currentSpeed = Input.GetKey(KeyCode.LeftShift)
            ? sprintSpeed
            : walkSpeed;

        // 3) Pewter boost?
        if (allomancer != null
            && allomancer.burningStatus.TryGetValue(MetalType.Pewter, out bool isPewterOn)
            && isPewterOn)
        {
            currentSpeed *= 1.2f;
        }

        // 4) Build direction vectors
        Vector3 forward = cameraTarget.forward;
        Vector3 right = cameraTarget.right;
        forward.y = right.y = 0;
        forward.Normalize();
        right.Normalize();

        // 5) Movement input
        Vector3 moveInput = Vector3.zero;
        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.W)) moveInput += forward;
            if (Input.GetKey(KeyCode.S)) moveInput -= forward;
            if (Input.GetKey(KeyCode.D)) moveInput += right;
            if (Input.GetKey(KeyCode.A)) moveInput -= right;
        }

        moveInput.Normalize();
        Vector3 newVelocity = rb.linearVelocity;
        if (isGrounded)
        {
            newVelocity.x = moveInput.x * currentSpeed;
            newVelocity.z = moveInput.z * currentSpeed;
        }
        rb.linearVelocity = newVelocity;

        // 6) Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }

        // 7) Iron/Steel burning logic
        bool burningIron = allomancer?.IsBurning(MetalType.Iron) ?? false;
        bool burningSteel = allomancer?.IsBurning(MetalType.Steel) ?? false;

        // Clear old lines
        foreach (var line in activeLines)
        {
            if (line != null)
                Destroy(line.gameObject);
        }
        activeLines.Clear();

        if (burningIron || burningSteel)
        {
            nearbyMetals.Clear();
            Collider[] hits = Physics.OverlapSphere(transform.position, metalDetectRadius, metalLayer);
            foreach (var hit in hits)
            {
                if (hit.attachedRigidbody != null)
                {
                    nearbyMetals.Add(hit.attachedRigidbody);

                    // Calculate distance and alpha
                    float dist = Vector3.Distance(rb.worldCenterOfMass, hit.attachedRigidbody.worldCenterOfMass);
                    float alpha = Mathf.Clamp01(1f - (dist / metalDetectRadius)); // Closer = more opaque

                    // Create a new LineRenderer GameObject
                    GameObject lineObj = new GameObject("MetalLine");
                    LineRenderer lr = lineObj.AddComponent<LineRenderer>();
                    lr.material = new Material(Shader.Find("Sprites/Default")); // Ensure material supports transparency
                    lr.startWidth = lr.endWidth = 0.05f;
                    lr.positionCount = 2;
                    lr.useWorldSpace = true;
                    lr.SetPosition(0, rb.worldCenterOfMass);
                    lr.SetPosition(1, hit.attachedRigidbody.worldCenterOfMass);

                    Color lineColor = new Color(0f, 0.5f, 1f, alpha); // Blue-ish with variable alpha
                    lr.startColor = lr.endColor = lineColor;

                    activeLines.Add(lr);
                }
            }

            if (Input.GetMouseButtonDown(0) && nearbyMetals.Count > 0)
            {
                Rigidbody closest = GetClosestMetal(nearbyMetals);
                if (closest != null)
                {
                    Vector3 dir = (closest.worldCenterOfMass - transform.position).normalized;

                    // Calculate clamped force based on player's weight
                    float playerWeight = rb.mass * Physics.gravity.magnitude;
                    float maxForce = playerWeight * 0.95f; // 95% of weight
                    float clampedForce = Mathf.Min(pushPullForce, maxForce);

                    if (burningIron)
                        closest.AddForce(-dir * clampedForce, ForceMode.Impulse); // Pull
                }
            } else if (Input.GetMouseButtonDown(1) && nearbyMetals.Count > 0)
            {
                Rigidbody closest = GetClosestMetal(nearbyMetals);
                if (closest != null)
                {
                    Vector3 dir = (closest.worldCenterOfMass - transform.position).normalized;

                    // Calculate clamped force based on player's weight
                    float playerWeight = rb.mass * Physics.gravity.magnitude;
                    float maxForce = playerWeight * 0.95f; // 95% of weight
                    float clampedForce = Mathf.Min(pushPullForce, maxForce);

                    if (burningSteel)
                        closest.AddForce(dir * clampedForce, ForceMode.Impulse); // Push
                }
            }
        }
    }

    Rigidbody GetClosestMetal(List<Rigidbody> metals)
    {
        Rigidbody closest = null;
        float minDist = float.MaxValue;

        foreach (var rb in metals)
        {
            float dist = Vector3.Distance(transform.position, rb.worldCenterOfMass);
            if (dist < minDist)
            {
                minDist = dist;
                closest = rb;
            }
        }

        return closest;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}