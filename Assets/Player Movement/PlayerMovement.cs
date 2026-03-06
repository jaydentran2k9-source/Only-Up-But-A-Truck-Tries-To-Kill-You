using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Components")]
    public Rigidbody rb;
    public Transform tf;

    [Header("Player Settings")]
    public float acceleration = 10f;
    public float topSpeed = 5f;
    public float runSpeed = 10f;

    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // Player direction
    private Vector3 move;
    private float sideInput;
    private float forwardInput;

    //Checks if the player has been knocked back
    private bool isKnockedBack;
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackDuration = 0.5f;

    /* Inserted configurable fields for tuning horizontal and vertical knockback */
    [SerializeField] private float horizontalKnockbackMultiplier = 1.5f;
    [SerializeField] private float upwardKnockbackBoost = 0.2f;

    // Grounded state
    private bool isGrounded = false;
    public bool isRunning = false;

    private PlatformTrigger1 currentPlatform;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tf = GetComponent<Transform>();
    }



    private void FixedUpdate()
    {
        // Update grounded state (safe-check groundCheck)
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
        }
        else
        {
            // fallback raycast if no groundCheck provided
            isGrounded = Physics.Raycast(tf.position, Vector3.down, 1.1f);
        }

        if (forwardInput == Mathf.Abs(1) && sideInput == Mathf.Abs(1))
        {
            Mathf.Sqrt(forwardInput);
            Mathf.Sqrt(sideInput);
        }
        move = tf.forward * forwardInput + tf.right * sideInput;
        Vector3 newVelocity = new Vector3(move.x * acceleration, 0, move.z * acceleration);
        rb.AddForce(newVelocity);
        Vector3 velocity = Vector3.ClampMagnitude(new(rb.linearVelocity.x, 0, rb.linearVelocity.z), topSpeed);
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;

        if (currentPlatform != null)
        {
            // Add the platform's velocity to the player's current velocity
            rb.linearVelocity += currentPlatform.velocity * Time.fixedDeltaTime;
        }
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        Vector2 v = ctx.ReadValue<Vector2>();
        print(v);
        sideInput = ctx.ReadValue<Vector2>().x;
        forwardInput = ctx.ReadValue<Vector2>().y;
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void Run(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isRunning = true;
            //topSpeed = runSpeed;
        }
        else if (ctx.canceled)
        {
            isRunning = false;
            //topSpeed = 5f; // reset to default walk speed
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("MovingPlatform"))
            currentPlatform = col.gameObject.GetComponent<PlatformTrigger1>();
    }

    private void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("MovingPlatform"))
            currentPlatform = null;
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (isKnockedBack) return;
        StartCoroutine(KnockbackRoutine(direction, force));
    }

    private IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        isKnockedBack = true;

        // Reset velocity for a consistent feel
        rb.linearVelocity = Vector3.zero;

        // Compute horizontal direction (XZ plane)
        Vector3 horizontalDir = new Vector3(direction.x, 0f, direction.z);
        if (horizontalDir.sqrMagnitude < 0.0001f)
        {
            // Fallback to player's forward direction projected onto XZ if input direction is nearly zero
            horizontalDir = new Vector3(tf.forward.x, 0f, tf.forward.z);
        }
        horizontalDir = horizontalDir.normalized;

        // Scale horizontal and vertical components separately
        Vector3 horizontalImpulse = horizontalDir * (force * horizontalKnockbackMultiplier);
        Vector3 verticalImpulse = Vector3.up * (upwardKnockbackBoost * force);

        // Final impulse combines boosted horizontal force and a small upward lift
        Vector3 finalImpulse = horizontalImpulse + verticalImpulse;

        rb.AddForce(finalImpulse, ForceMode.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        isKnockedBack = false;
    }

}
