using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPush : MonoBehaviour
{
    // Public variables that can be adjusted in the Unity Inspector
    public float pushForce = 100f; // Force to push the other player
    public float pushUpForce = 50f; // Upward force to push the other player
    public float raycastDistance = 100f; // Distance for the raycast

    [SerializeField] private GameObject playerPushLocation;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            RaycastHit hit;
            Debug.Log("B pressed");

            if (Physics.Raycast(playerPushLocation.transform.position, playerPushLocation.transform.forward, out hit, raycastDistance) && hit.collider.CompareTag("Player"))
            {
                Debug.Log("hit");
                Debug.DrawRay(playerPushLocation.transform.position, playerPushLocation.transform.forward, Color.red, 2);
#if UNITY_EDITOR
                    //EditorGUIUtility.PingObject(hit.collider.gameObject);
#endif
                PushPlayer(hit.collider.gameObject);
            }
            
        }
        
        /*
        // Check if the right mouse button is pressed
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit; // Variable to store information about what the raycast hits
                        //Vector3 forward = transform.TransformDirection(Vector3.forward) * raycastDistance;

        // Perform the raycast
        if (Physics.Raycast(ray, out hit))
        {
            //EditorGUIUtility.PingObject(hit.collider.gameObject);
            //EditorGUIUtility.PingObject(gameObject);
            Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2.0f);
            Debug.Log("Is the raycast hit the own player object?" + (hit.collider.gameObject == gameObject));

            
            // Check if the hit object is another player
            if (hit.collider.CompareTag("Player") && Input.GetKeyDown(KeyCode.B))
            {
                PushPlayer(hit.collider.gameObject);
            }
        }*/

    }

    // Method to apply forces to push the other player
    void PushPlayer(GameObject player)
    {
        Debug.Log("push player called");
        Rigidbody rb = player.GetComponentInParent<Rigidbody>();

        if (rb != null)
        {
            Debug.Log("rb not null adding force");
            Debug.Log("player.transformp osition: " + player.transform.position);
            Debug.Log("transform.position: " + transform.position);
            Vector3 pushDirection = (player.transform.position - transform.position).normalized;

            PushPlayerClientRpc(rb.gameObject.GetComponent<NetworkObject>().NetworkObjectId, pushDirection);

            //rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            //rb.AddForce(Vector3.up * pushUpForce, ForceMode.Impulse);
        }
    }

    [ClientRpc()]
    private void PushPlayerClientRpc(ulong networkObjectId, Vector3 pushDirection)
    {
        Debug.Log("push player client rpc called");
        GameObject playerToPush = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject;
        Debug.Log("player top push null ? " + playerToPush == null);

#if UNITY_EDITOR
        EditorGUIUtility.PingObject(playerToPush);
#endif

        playerToPush.GetComponent<Rigidbody>().AddForce(pushDirection * pushForce, ForceMode.Impulse);
        playerToPush.GetComponent<Rigidbody>().AddForce(Vector3.up * pushUpForce, ForceMode.Impulse);
    }

}