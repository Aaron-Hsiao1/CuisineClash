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

        if (rb.velocity.y < 0)
        {
            //Debug.Log("rb.velocity.y < 0 : " + rb.velocity.y);
            //rb.velocity += Vector3.up * (Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
            //rb.velocity += new Vector3(0f, Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime, 0f);
            //_verticalVelocity += -25f * Time.deltaTime;
        }

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);

        MyInput();
        SpeedControl();


        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
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

        //Debug.Log(_verticalVelocity);
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
        //calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f + new Vector3(0.0f, _verticalVelocity, 0.0f), ForceMode.Force);

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f + new Vector3(0.0f, _verticalVelocity), ForceMode.Force);
        }
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f /** airMultiplier*/ + new Vector3(0.0f, _verticalVelocity), ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);

        //limits velocity
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        //rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * -9.81f);
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

    public float GetVerticalInput()
    {
        return verticalInput;
    }

    public float GetHorizontalInput()
    {
        return horizontalInput;
    }

}