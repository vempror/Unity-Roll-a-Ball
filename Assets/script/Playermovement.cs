using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PlayerAutoRunner : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField] float startForwardSpeed = 6f;
    [SerializeField] float maxForwardSpeed = 25f;
    [SerializeField] float speedIncreaseRate = 0.5f;
    [SerializeField] float strafeSpeed = 7f;
    [SerializeField] float acceleration = 30f;

    [Header("Ground & Bounds")]
    [SerializeField] float groundCheckDistance = 0.6f;
    [SerializeField] LayerMask groundMask = ~0;
    [SerializeField] float groundSnapForce = 20f;
    [SerializeField] float clampXRange = 5f; // limit how far player can move left/right

    [Header("Orientation")]
    [SerializeField] bool faceForward = true;
    [SerializeField] Transform forwardSource;

    Rigidbody rb;
    float currentForwardSpeed;
    bool isGrounded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.freezeRotation = true;
        rb.constraints &= ~(RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ);

        currentForwardSpeed = startForwardSpeed;
    }

    void FixedUpdate()
    {
        // --- Speed ramp ---
        currentForwardSpeed = Mathf.MoveTowards(currentForwardSpeed, maxForwardSpeed, speedIncreaseRate * Time.fixedDeltaTime);

        // --- Input ---
        float x = 0f;
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) x -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
        }
        else
        {
            x = Input.GetAxisRaw("Horizontal");
        }

        // --- Directions ---
        Vector3 fwd = forwardSource ? forwardSource.forward : transform.forward;
        fwd.y = 0f; fwd.Normalize();
        Vector3 right = Vector3.Cross(Vector3.up, fwd).normalized; // ✅ Correct direction

        // --- Desired move ---
        Vector3 desiredXZ = fwd * currentForwardSpeed + right * (x * strafeSpeed);
        Vector3 currentXZ = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        Vector3 nextXZ = Vector3.MoveTowards(currentXZ, desiredXZ, acceleration * Time.fixedDeltaTime);

        // --- Ground check ---
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, groundCheckDistance, groundMask);
        if (isGrounded && rb.linearVelocity.y < 0f)
            rb.AddForce(Vector3.down * groundSnapForce, ForceMode.Acceleration);

        // --- Apply velocity ---
        rb.linearVelocity = new Vector3(nextXZ.x, rb.linearVelocity.y, nextXZ.z);

        // --- Face forward ---
        if (faceForward && fwd.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(fwd, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, 12f * Time.fixedDeltaTime));
        }

        // --- Clamp side position so player can’t fall off edges ---
        Vector3 pos = rb.position;
        pos.x = Mathf.Clamp(pos.x, -clampXRange, clampXRange);
        rb.position = pos;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.2f, Vector3.down * groundCheckDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(-clampXRange, transform.position.y, transform.position.z),
                        new Vector3(clampXRange, transform.position.y, transform.position.z));
    }
}
