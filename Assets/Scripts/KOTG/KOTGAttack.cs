using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class KOTGAttack : NetworkBehaviour
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
        if (Input.GetKey(KeyCode.Mouse0) && !isDashing && !isCooldown && (SceneManager.GetActiveScene().name == "KingOfTheGrill"))//&& (SceneManager.GetActiveScene().name == "KingOfTheGrill")
        {
            ChargeDash();
            isCharging = true; //for animation
        }
        UpdateDashDirection();
        // If Space is released, execute the dash
        if (Input.GetKeyUp(KeyCode.Mouse0) && !isDashing && !isCooldown && (SceneManager.GetActiveScene().name == "KingOfTheGrill"))//&& (SceneManager.GetActiveScene().name == "KingOfTheGrill")
        {
            ExecuteDash();
            isCharging = false; //for animation
        }
        //UpdateDashDirection();

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
            //Debug.Log("chargining up");
        }
        else
        {
            //Debug.Log("Max charge");
        }
    }

    private void ExecuteDash()
    {
        // Calculate the dash force based on how much it has been charged
        dashForce = Mathf.Lerp(dashSpeed, maxDashForce, dashChargeTime / maxDashChargeTime);

        // Start cooldown for dash
        isDashing = true;
        Dash();
        StartCoroutine(DashCooldown());
        isCooldown = true;
        timeSinceLastAction = 0f;
    }

    private IEnumerator DashCooldown()
    {
        // Wait for the duration of the dash
        yield return new WaitForSeconds(dashDuration);
        moveScript.SetLaunching(false);
        // Reset after dash is finished
        if (isDashing)
        {
            isDashing = false;
        }

        dashChargeTime = 0f; // Reset the charge
    }
    private void UpdateDashDirection()
    {
        // The dash direction is the player's forward direction (relative to their current rotation)
        dashDirection = Orientation.transform.forward; // Direction the player is currently facing
    }
    void Dash()
    {
        //Debug.Log("Dash direction: " + cameraForward);
        moveScript.SetLaunching(true);
        rb.AddForce(cameraForward * dashForce * 5f, ForceMode.Impulse);
        //rb.velocity = cameraForward * dashForce * DashScale;
    }

    /*private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("KOTGAttackCollider"))
        {
            Debug.Log("COllided with kotg collider");
        }
        // If the dash is active, apply knockback to the object collided with
        if (isDashing)
        {
            isDashing = false;
            Debug.Log("oncollisionenter isDashing = false");
            // Calculate knockback force based on the charge time
            float knockbackForce = Mathf.Lerp(0, knockbackMultiplier, dashChargeTime / maxDashChargeTime);
            // Apply knockback force to the collided object
            Rigidbody hitRb = collision.gameObject.GetComponent<Rigidbody>(); // Get Rigidbody of the object hit
            if (hitRb.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                return;
            }

            if (hitRb != null)
            {

                Vector3 knockbackDirection = collision.transform.position - transform.position;
                knockbackDirection.Normalize();
                Debug.Log("Attackign: " + hitRb.GetComponent<NetworkObject>().OwnerClientId);

                if (IsHost)
                {
                    AttackPlayerClientRpc(hitRb.GetComponent<NetworkObject>().NetworkObjectId, knockbackDirection * knockbackForce);
                }
                else if (IsClient)
                {
                    AttackPlayerServerRpc(hitRb.GetComponent<NetworkObject>().NetworkObjectId, knockbackDirection * knockbackForce);
                }


                AttackPlayerClientRpc(hitRb.GetComponent<NetworkObject>().NetworkObjectId, knockbackDirection * knockbackForce);

            }
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        if (isDashing && other.gameObject.CompareTag("KOTGAttackCollider") && other.gameObject.GetComponentInParent<NetworkObject>().OwnerClientId != NetworkManager.Singleton.LocalClientId)
        {
            isDashing = false;
            // Check if the other object has a Rigidbody (this is important to apply force)
            if (other.GetComponentInParent<Rigidbody>() != null)
            {
                float knockbackForce = Mathf.Lerp(0, knockbackMultiplier, dashChargeTime / maxDashChargeTime);
                Rigidbody rb = other.GetComponentInParent<Rigidbody>();

                // Calculate the direction from the center of the area to the object's position
                Vector3 flingDirection = (other.transform.position - transform.position).normalized;

                // Apply force to fling the object away from the area
                if (IsHost)
                {
                    AttackPlayerClientRpc(rb.GetComponent<NetworkObject>().NetworkObjectId, flingDirection * knockbackForce);
                }
                else if (IsClient)
                {
                    AttackPlayerServerRpc(rb.GetComponent<NetworkObject>().NetworkObjectId, flingDirection * knockbackForce);
                }
                Debug.Log("flinging");
            }
        }
    }

    private IEnumerator LaunchPlayer(GameObject player, Vector3 totalForce, Rigidbody rb)
    {
        player.gameObject.GetComponentInParent<PlayerMovement>().SetLaunching(true);
        rb.velocity = new Vector3(0, 0, 0);
        rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
        rb.AddForce(totalForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);
        player.gameObject.GetComponentInParent<PlayerMovement>().SetLaunching(false);
    }

    [ClientRpc()]
    private void AttackPlayerClientRpc(ulong networkObjectId, Vector3 pushDirection) //called on player that gets the force applied to them
    {
        GameObject playerToPush = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject;

        if (playerToPush.GetComponent<NetworkObject>().OwnerClientId != NetworkManager.Singleton.LocalClientId)
        {
            return;
        }
        StartCoroutine(LaunchPlayer(playerToPush, pushDirection.normalized * 25f, playerToPush.GetComponent<Rigidbody>()));
        //playerToPush.GetComponent<Rigidbody>().AddForce(pushDirection.normalized * 100f, ForceMode.Impulse);
    }

    [ServerRpc(RequireOwnership = false)]
    private void AttackPlayerServerRpc(ulong networkObjectId, Vector3 pushDirection) //called on player that gets the force applied to them
    {
        GameObject playerToPush = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject;

        if (playerToPush.GetComponent<NetworkObject>().OwnerClientId != NetworkManager.Singleton.LocalClientId)
        {
            return;
        }
        StartCoroutine(LaunchPlayer(playerToPush, pushDirection.normalized * 25f, playerToPush.GetComponent<Rigidbody>()));
        //playerToPush.GetComponent<Rigidbody>().AddForce(pushDirection.normalized * 100f, ForceMode.Impulse);
    }
}
