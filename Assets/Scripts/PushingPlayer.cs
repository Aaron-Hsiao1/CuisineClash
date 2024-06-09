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
    public float pushForce = 2f; // Force to push the other player
    public float pushUpForce = 2f; // Upward force to push the other player
    public float raycastDistance = 10f; // Distance for the raycast
    public float pushDistanceThreshold = 3f;

    public GameObject playerCam;
    
     void Update()
    {
        // Check if the B key is pressed
        if (Input.GetKeyDown(KeyCode.B))
        {
            // Create a ray from the main camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit; // Variable to store information about what the raycast hits

            // Perform the raycast
            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                

                // Check if the hit object is another player
                if (hit.collider.CompareTag("Player"))
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, hit.collider.gameObject.transform.position);
                    Rigidbody rb = hit.collider.gameObject.GetComponentInParent<Rigidbody>();
                        
                        if (rb != null)
                        {
                             if (distanceToPlayer <= pushDistanceThreshold && distanceToPlayer != 0)
                            {
                                
                                Vector3 pushDirection = (hit.collider.gameObject.transform.position - transform.position).normalized;
                                pushDirection.y = 0; // Neutralize the vertical component   
                                rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                                rb.AddForce(Vector3.up * pushUpForce, ForceMode.Impulse);
                        }
                    }
                }
            }
        }
    }
}
