using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPush : NetworkBehaviour
{
    // Public variables that can be adjusted in the Unity Inspector
    public float pushForce = 100f; // Force to push the other player
    public float pushUpForce = 2f; // Upward force to push the other player
    public float raycastDistance = 100f; // Distance for the raycast
    public float pushCooldown = 0.5f;

    private bool canPush = true;

    [SerializeField] private GameObject playerPushLocation;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && canPush)
        {
            RaycastHit hit;
            canPush = false;
            StartCoroutine(ResetPlayerCooldown());
            Debug.Log("B pressed");

            if (Physics.Raycast(playerPushLocation.transform.position, playerPushLocation.transform.forward, out hit, raycastDistance) && hit.collider.CompareTag("PlayerPush"))
            {
                Debug.Log("hit");
                Debug.DrawRay(playerPushLocation.transform.position, playerPushLocation.transform.forward, Color.red, 2);

                PushPlayer(hit.collider.gameObject);
            }
            
        }
    }

    void PushPlayer(GameObject player)
    {
        Debug.Log("push player called");
        Rigidbody rb = player.GetComponentInParent<Rigidbody>();

        if (rb != null)
        {
            Vector3 pushDirection = (player.transform.position - transform.position).normalized;

            if (IsHost)
            {
                PushPlayerClientRpc(rb.gameObject.GetComponent<NetworkObject>().NetworkObjectId, pushDirection);
            }
            else if (IsClient)
            {
                PushPlayerServerRpc(rb.gameObject.GetComponent<NetworkObject>().NetworkObjectId, pushDirection);
            }
            

            //rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            //rb.AddForce(Vector3.up * pushUpForce, ForceMode.Impulse);
        }
    }

    [ClientRpc()]
    private void PushPlayerClientRpc(ulong networkObjectId, Vector3 pushDirection)
    {
        Debug.Log("push player client rpc called");
        GameObject playerToPush = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject;
        if (playerToPush.GetComponent<NetworkObject>().OwnerClientId != NetworkManager.Singleton.LocalClientId)
        {
            return;
        }

        playerToPush.GetComponent<Rigidbody>().AddForce(pushDirection * pushForce, ForceMode.Impulse);
        playerToPush.GetComponent<Rigidbody>().AddForce(Vector3.up * pushUpForce, ForceMode.Impulse);
    }

    [ServerRpc()]
    private void PushPlayerServerRpc(ulong networkObjectId, Vector3 pushDirection)
    {
        GameObject playerToPush = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject;

        playerToPush.GetComponent<Rigidbody>().AddForce(pushDirection * pushForce, ForceMode.Impulse);
        playerToPush.GetComponent<Rigidbody>().AddForce(Vector3.up * pushUpForce, ForceMode.Impulse);
    }

    public IEnumerator ResetPlayerCooldown()
    {
        yield return new WaitForSeconds(pushCooldown);
        canPush = true;
    }

}