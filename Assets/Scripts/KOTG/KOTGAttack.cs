using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KOTGAttack : MonoBehaviour
{
    public float DashScale = 5f;
    private float dashTime = 0f;
    private float dashForce;
    public bool isCharging;

    public Transform cameraTransform; // Reference to the camera
    private Vector3 cameraForward;

    private bool isCooldown = false;
    public float cooldownTime = 1;
    private float timeSinceLastAction = 0f;

    public Transform Orientation;
    // Start is called before the first frame update
    public float dashSpeed = 15f; // Base speed of the dash
    public float maxDashChargeTime = 5f; // Max time to charge the dash
    public float dashChargeSpeed = 1f; // Speed at which the dash strength increases when charging
    public float maxDashForce = 30f; // Max force when the dash is fully charged
    public float dashDuration = 2f; // Duration of the dash
    public float knockbackMultiplier = 5f; // Multiplier for knockback based on charge time

    [SerializeField] private PlayerMovement moveScript;

    public bool isDashing = false; // To check if the dash is currently active
    private float dashChargeTime = 0f; // Time for which dash has been charged
    private Rigidbody rb; // Reference to Rigidbody for movement
    private Vector3 dashDirection;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component for movement
    }

    // Update is called once per frame
    void Update()
    {
        cameraForward = cameraTransform.forward;
        // Input to start charging the dash (holding space bar)
        if (Input.GetKey(KeyCode.Mouse0) && !isDashing && !isCooldown && (SceneManager.GetActiveScene().name != "KingOfTheGrill"))
        {
            ChargeDash();
            isCharging = true; //for animation
        }

        // If Space is released, execute the dash
        if (Input.GetKeyUp(KeyCode.Mouse0) && !isDashing && !isCooldown && (SceneManager.GetActiveScene().name != "KingOfTheGrill"))
        {
            ExecuteDash();
            isCharging = false; //for animation
        }
        UpdateDashDirection();

        if (isDashing)
        {
            Dash();
        }

         if (isCooldown)
        {
            timeSinceLastAction += Time.deltaTime; // Increase the time by the time passed since last frame

            if (timeSinceLastAction >= cooldownTime)
            {
                isCooldown = false; // Cooldown is over
                timeSinceLastAction = 0f; // Reset the timer
            }
        }
    }

    private void ChargeDash()
    {
        if (dashChargeTime < maxDashChargeTime)
        {
            dashChargeTime += dashChargeSpeed * Time.deltaTime; // Increase charge time
            Debug.Log("chargining up");
        }
        else
        {
            Debug.Log("Max charge");
        }
    }

    private void ExecuteDash()
    {
        moveScript.launching = true;
        // Calculate the dash force based on how much it has been charged
        dashForce = Mathf.Lerp(dashSpeed, maxDashForce, dashChargeTime / maxDashChargeTime);

        // Start cooldown for dash
        isDashing = true;
        StartCoroutine(DashCooldown());
        isCooldown = true;
        timeSinceLastAction = 0f;
    }

    private System.Collections.IEnumerator DashCooldown()
    {
        // Wait for the duration of the dash
        yield return new WaitForSeconds(dashDuration);
        // Reset after dash is finished
        moveScript.launching = false;
        isDashing = false;
        dashChargeTime = 0f; // Reset the charge
    }
    private void UpdateDashDirection()
    {
        // The dash direction is the player's forward direction (relative to their current rotation)
        dashDirection = Orientation.transform.forward; // Direction the player is currently facing
    }
    void Dash() 
    {
        rb.velocity = cameraForward * dashForce * DashScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If the dash is active, apply knockback to the object collided with
        if (isDashing)
        {
            // Calculate knockback force based on the charge time
            float knockbackForce = Mathf.Lerp(0, knockbackMultiplier, dashChargeTime / maxDashChargeTime);

            // Apply knockback force to the collided object
            Rigidbody hitRb = collision.gameObject.GetComponent<Rigidbody>(); // Get Rigidbody of the object hit
            if (hitRb != null)
            {
                /*
                IHitable hitable = collision.gameObject.GetComponent<IHitable>();
                {
                    if(hitable != null)
                    {
                        hitable.Execute(transform);
                    }
                }
                */
                
                Vector3 knockbackDirection = collision.transform.position - transform.position;
                knockbackDirection.Normalize();

                hitRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
                
            }
        }
    }
}
