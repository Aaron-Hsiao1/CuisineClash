using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoKartMovement : MonoBehaviour
{
    public float acceleration = 10f;
    public float maxSpeed = 20f;
    public float steeringSpeed = 2f;
    public float driftForce = 5f;
    public float driftBoostForce = 10f;
    public float driftBoostDuration = 2f;
    public LayerMask groundLayer;
    public float groundDrag = 5f;

    private Rigidbody rb;
    private bool isDrifting = false;
    private bool driftBoostReady = false;
    private float driftTime = 0f;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        float forwardInput = Input.GetAxisRaw("Vertical"); // W/S or Up/Down arrow
        float steeringInput = Input.GetAxisRaw("Horizontal"); // A/D or Left/Right arrow

        moveDirection = transform.forward * forwardInput;

        // Steering
        if (!isDrifting && Mathf.Abs(steeringInput) > 0 && rb.velocity.magnitude > 1f)
        {
            float rotation = steeringInput * steeringSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up * rotation);
        }

        // Start drifting
        if (Input.GetKeyDown(KeyCode.LeftShift) && Mathf.Abs(steeringInput) > 0)
        {
            StartDrift(steeringInput);
        }

        // End drifting
        if (Input.GetKeyUp(KeyCode.LeftShift) && isDrifting)
        {
            EndDrift();
        }
    }

    void FixedUpdate()
    {
        // Acceleration
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

    void StartDrift(float steeringInput)
    {
        isDrifting = true;
        driftBoostReady = true;
        driftTime = 0f;

        float driftDirection = steeringInput > 0 ? 1f : -1f;
        rb.AddForce(transform.right * driftDirection * driftForce, ForceMode.Acceleration);
    }

    void EndDrift()
    {
        isDrifting = false;

        if (driftBoostReady && driftTime >= 0.5f)
        {
            driftBoostReady = false;
            StartCoroutine(DriftBoost());
        }
    }

    IEnumerator DriftBoost()
    {
        float originalMaxSpeed = maxSpeed;
        maxSpeed += driftBoostForce;

        yield return new WaitForSeconds(driftBoostDuration);

        maxSpeed = originalMaxSpeed;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.2f, groundLayer);
    }
}