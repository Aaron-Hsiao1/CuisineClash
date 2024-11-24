using Unity.Netcode;
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

    public float horizontalInput;
    public float verticalInput;

    private float _verticalVelocity;
    public float JumpHeight = 1.5f;
    private float _terminalVelocity = 53.0f;

    [SerializeField] private float fallMultiplier = 50f;

    Vector3 moveDirection;

    Rigidbody rb;

    public KeyCode jumpKey = KeyCode.Space;

    // Boolean to indicate whether speed is currently boosted
    private bool isSpeedBoosted = false;

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

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);

        MyInput();
        SpeedControl();

        // apply gravity over time if under terminal
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
        if (!isSpeedBoosted) // Only adjust speed if it's not boosted
        {
            Sprint();
        }

        if (Input.GetKey(jumpKey) && canJump && grounded)
        {
            canJump = false;
            Jump();
            Invoke(nameof(resetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        //calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f + new Vector3(0.0f, _verticalVelocity, 0.0f), ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * -9.81f);
    }

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = 10;
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

    public float GetVerticalInput()
    {
        return verticalInput;
    }

    public float GetHorizontalInput()
    {
        return horizontalInput;
    }

    // Set moveSpeed and update the isSpeedBoosted status
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
        isSpeedBoosted = (newSpeed != 5f); // true if boosted speed
    }
}
