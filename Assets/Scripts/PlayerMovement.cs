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

    private float _verticalVelocity;
    public float JumpHeight = 1.5f;
    private float _terminalVelocity = 53.0f;

    [SerializeField] private float fallMultiplier = 50f;

    Vector3 moveDirection;
    Rigidbody rb;

    public KeyCode jumpKey = KeyCode.Space;

    private float maxSlopeAngle = 45f; // Maximum slope the player can walk on

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
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

        // Apply custom gravity if falling
        if (!grounded && rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        // Prevent velocity from going beyond terminal velocity
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += -11f * Time.deltaTime;
        }

        if (grounded)
        {
            rb.drag = groundDrag;
            _verticalVelocity = 0f;
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = -2f;
            }
        }
        else
        {
            rb.drag = 0;
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

        if (OnSlope())
        {
            // Move along the slope
            Vector3 slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, GetSlopeNormal());
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f + new Vector3(0.0f, _verticalVelocity, 0.0f), ForceMode.Force);
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
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * -9.81f);
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
