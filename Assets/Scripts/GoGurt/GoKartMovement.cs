using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoKartMovement : NetworkBehaviour
{
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float steeringSpeed;
    [SerializeField] private float driftForce;
    [SerializeField] private float driftBoostForce;
    [SerializeField] private float driftBoostDuration;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundDrag;

    private Rigidbody rb;
    private bool isDrifting = false;
    private bool driftBoostReady = false;
    private float driftTime = 0f;
    private Vector3 moveDirection;

    public KeyCode driftKey = KeyCode.LeftShift;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        float forwardInput = Input.GetAxisRaw("Vertical"); // W/S or Up/Down arrow
        float steeringInput = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrow

        moveDirection = transform.forward * forwardInput;

        // Steering
        if (!isDrifting && Mathf.Abs(steeringInput) > 0)
        {
            float rotation = steeringInput * steeringSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up * rotation);
        }

        // Start drifting
        if (Input.GetKeyDown(driftKey) && Mathf.Abs(steeringInput) > 0)
        {
            StartDrift(steeringInput);
        }

        // End drifting
        if (Input.GetKeyUp(driftKey) && isDrifting)
        {
            EndDrift();
        }
    }

    private void FixedUpdate()
    {
        if (!IsLocalPlayer)
        {
            return;
        }

        MoveKart();
    }

    private void MoveKart()
    {
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(moveDirection * acceleration, ForceMode.Acceleration);
        }

        // Apply ground drag
        if (IsGrounded())
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void StartDrift(float steeringInput)
    {
        isDrifting = true;
        driftBoostReady = true;
        driftTime = 0f;

        float driftDirection = steeringInput > 0 ? 1f : -1f;
        rb.AddForce(transform.right * driftDirection * driftForce, ForceMode.Acceleration);
    }

    private void EndDrift()
    {
        isDrifting = false;

        if (driftBoostReady && driftTime >= 0.5f)
        {
            driftBoostReady = false;
            StartCoroutine(DriftBoost());
        }
    }

    private IEnumerator DriftBoost()
    {
        float originalMaxSpeed = maxSpeed;
        maxSpeed += driftBoostForce;

        yield return new WaitForSeconds(driftBoostDuration);

        maxSpeed = originalMaxSpeed;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.2f, groundLayer);
    }
}