using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public Transform cameraTransform;
    public float speed = 5f;
    public float rotationSpeed = 10f;

    public float gravity = -20f;
    public float jumpHeight = 1.2f;

    // These two fix the "sometimes jump doesn't work" feeling
    public float coyoteTime = 0.12f;      // jump shortly after leaving ground
    public float jumpBufferTime = 0.12f;  // jump shortly before landing

    private CharacterController controller;
    private Vector3 velocity;

    private float coyoteTimer;
    private float jumpBufferTimer;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // --- Input (WASD) ---
        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) input.y += 1;
            if (Keyboard.current.sKey.isPressed) input.y -= 1;
            if (Keyboard.current.aKey.isPressed) input.x -= 1;
            if (Keyboard.current.dKey.isPressed) input.x += 1;

            // Jump buffer: remember jump press for a short time
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
                jumpBufferTimer = jumpBufferTime;
        }

        // Decrease buffer timer over time
        if (jumpBufferTimer > 0f)
            jumpBufferTimer -= Time.deltaTime;

        // --- Camera-relative movement ---
        Vector3 move = Vector3.zero;
        if (cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0;
            camRight.y = 0;

            move = camForward.normalized * input.y + camRight.normalized * input.x;
        }

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        controller.Move(move * speed * Time.deltaTime);

        // --- Grounding + coyote time ---
        bool grounded = controller.isGrounded;

        if (grounded)
        {
            coyoteTimer = coyoteTime;

            // Keeps you stuck to ground instead of hovering
            if (velocity.y < 0f) velocity.y = -2f;
        }
        else
        {
            coyoteTimer -= Time.deltaTime;
        }

        // --- Jump: if jump was pressed recently AND we are grounded (or within coyote time) ---
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpBufferTimer = 0f; // consume the buffered jump
            coyoteTimer = 0f;
        }

        // --- Gravity ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}