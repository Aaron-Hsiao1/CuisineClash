using System.Collections;
using UnityEngine;

public class DashMechanic : MonoBehaviour
{
    PlayerMovement moveScript;
    public float dashSpeed;
    public float dashTime;
    public float cooldownTime = 5f;
    private float lastUsedTime; 



    private void Start()
    {
        moveScript = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        // Check if the Q key is pressed and the dash is not on cooldown
        if (Input.GetKeyDown(KeyCode.Q) && Time.time > lastUsedTime + cooldownTime)
        {
            StartCoroutine(Dash());
            lastUsedTime = Time.time;
        }
    }

    IEnumerator Dash()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        float startTime = Time.time;

        // Get dash direction from player movement input
        Vector3 dashDirection = moveScript.orientation.forward * moveScript.GetVerticalInput() +
                                moveScript.orientation.right * moveScript.GetHorizontalInput();
        dashDirection = dashDirection.normalized;

        // Perform the dash
        while (Time.time < startTime + dashTime)
        {
            rb.velocity = dashDirection * dashSpeed;
            yield return null;
        }

        rb.velocity = Vector3.zero; // Stop movement after the dash
    }
}
