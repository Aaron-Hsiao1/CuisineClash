using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float hasPotatoSpeedMultiplier;

    [SerializeField] private float groundDrag;

    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask Ground;
    [SerializeField] private bool grounded;

    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] private bool canJump;

    [SerializeField] private Transform orientation;

    [SerializeField] private float horizontalInput;
    [SerializeField] private float verticalInput;

    private float _verticalVelocity;
    [SerializeField] private float JumpHeight = 1.5f;
    private float _terminalVelocity = 53.0f;

    [SerializeField] private float fallMultiplier = 50f;

    [SerializeField] private Vector3 moveDirection;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private FreeCam freeCam;

    public KeyCode jumpKey = KeyCode.Space;

	//public KOTGAttack KOTGA;

	[SerializeField] private HotPotatoManager hotPotatoManager;
    private bool launching = false;

    public override void OnNetworkSpawn()
    {
        hasPotatoSpeedMultiplier = 1.25f;
        walkSpeed = 5f;

        if (SceneManager.GetActiveScene().name == "HotPotato")
        {
            hotPotatoManager = GameObject.Find("Hot Potato Manager").GetComponent<HotPotatoManager>();
        }
    }


    void Update()
    {
        if (!IsLocalPlayer || freeCam.looking == true)
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
        if (!launching)
        {
            SpeedControl();
        }
        //Debug.Log("Launching: " + launching);
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
            Invoke(nameof(ResetJump), jumpCooldown);
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
        Debug.Log("jumping");
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * -9.81f);
    }

    private void Sprint()
    {
        float  targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        if (SceneManager.GetActiveScene().name == "HotPotato" && NetworkManager.Singleton.LocalClientId == hotPotatoManager.currentPlayerWithPotato.Value)
        {
            moveSpeed *= hasPotatoSpeedMultiplier;
        }

        moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, Time.deltaTime * 10f);
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }


    private void ResetJump()
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

    public Transform GetOrientation()
    {
        return orientation;
    }

    public bool IsGrounded()
    {
        return grounded;
    }

    public void SetLaunching(bool launching)
    {
        this.launching = launching;
    }
}