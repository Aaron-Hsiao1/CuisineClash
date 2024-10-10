using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    public float moveSpeed;
    public float sprintSpeed;

    public float groundDrag;
    public float playerHeight;
    public LayerMask Ground;
    bool grounded;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool canJump;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    public float JumpHeight = 1.5f;

    [SerializeField] private float fallMultiplier = 2.5f; // Adjust for faster fall
    [SerializeField] private float lowJumpMultiplier = 2f; // Adjust for lower jumps
    [SerializeField] private float normalJumpGravityMultiplier = 1f; // Normal gravity during ascent

    Vector3 moveDirection;
    Rigidbody rb;

    public KeyCode jumpKey = KeyCode.Space;

    private float maxSlopeAngle = 45f; // Maximum slope the player can walk on

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.mass = 1f;  // Set player mass
        canJump = true;
    }

    void Update()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        // Check if player is grounded
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);

        MyInput();
        SpeedControl();

        // Apply custom gravity only if airborne
        if (!grounded)
        {
            ApplyGravity(); // Handle gravity separately only when not grounded
        }
    }

    private void FixedUpdate()
    {
        if (!IsLocalPlayer)
        {
            return;
        }
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        Sprint();

        if (Input.GetKey(jumpKey) && canJump && grounded)
        {
            canJump = false;
            Jump();
            Invoke(nameof(resetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (grounded)
        {
            // Apply movement force only when grounded
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!grounded)
        {
            // Apply movement force with air multiplier when in the air
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Limit velocity
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        if (grounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset any vertical velocity
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); // Apply jump force
        }
    }

    private void ApplyGravity()
    {
        if (rb.velocity.y > 0) // Player is ascending
        {
            if (!Input.GetKey(jumpKey))
            {
                // Weaker gravity for lower jumps if jump key is released
                rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
            else
            {
                // Normal gravity while ascending
                rb.velocity += Vector3.up * Physics.gravity.y * (normalJumpGravityMultiplier - 1) * Time.deltaTime;
            }
        }
        else if (rb.velocity.y < 0) // Player is falling
        {
            // Gradually increase gravity for smoother descent
            float fallSpeed = Mathf.Lerp(1f, fallMultiplier, Time.deltaTime * 2f); // Gradual fall increase
            rb.velocity += Vector3.up * Physics.gravity.y * (fallSpeed - 1) * Time.deltaTime;
        }
    }

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = sprintSpeed;
        }
        else
        {
            moveSpeed = 5;
        }
    }

    private void resetJump()
    {
        canJump = true;
    }

    private bool OnSlope()
    {
        if (grounded)
        {
            RaycastHit slopeHit;
            if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
            {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
                return angle < maxSlopeAngle && angle != 0;
            }
        }
        return false;
    }

    private Vector3 GetSlopeNormal()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, playerHeight * 0.5f + 0.3f))
        {
            return hit.normal;
        }
        return Vector3.up;
    }

    public float GetVerticalInput()
    {
        return verticalInput;
    }

    public float GetHorizontalInput()
    {
        return horizontalInput;
    }
}
