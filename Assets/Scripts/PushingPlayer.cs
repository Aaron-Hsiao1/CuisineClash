using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPush : MonoBehaviour
{
    // Public variables that can be adjusted in the Unity Inspector
    public float pushForce = 10f; // Force to push the other player
    public float pushUpForce = 5f; // Upward force to push the other player
    public float raycastDistance = 100f; // Distance for the raycast

    void Update()
    {
        // Check if the right mouse button is pressed
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; // Variable to store information about what the raycast hits
                        //Vector3 forward = transform.TransformDirection(Vector3.forward) * raycastDistance;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2.0f);
            // Check if the hit object is another player
            if (hit.collider.CompareTag("Player") && Input.GetKeyDown(KeyCode.B))
            {
                Debug.Log("raycast is hit");
                PushPlayer(hit.collider.gameObject);
            }
        }

    }

    // Method to apply forces to push the other player
    void PushPlayer(GameObject player)
    {
        Rigidbody rb = player.GetComponentInParent<Rigidbody>();

        if (rb != null)
        {
            Vector3 pushDirection = (player.transform.position - transform.position).normalized;
            pushDirection.y = 0; // Neutralize the vertical component
            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            rb.AddForce(Vector3.up * pushUpForce, ForceMode.Impulse);
        }
    }
}
