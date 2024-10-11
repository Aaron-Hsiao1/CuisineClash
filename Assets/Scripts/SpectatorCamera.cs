using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpectatorCamera : NetworkBehaviour
{
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private ulong targetClientId;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdatePosition();
    }

    public void SetCameraPositionAndRotation(Vector3 position, Quaternion rotation)
    {
        targetPosition = position;
        targetRotation = rotation;
    }
    private void UpdatePosition()
    {
        SendClientPositionServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    public void SetTargetClientId(ulong targetClientId)
    {
        this.targetClientId = targetClientId;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendClientPositionServerRpc(ulong clientId, ServerRpcParams serverRpcParams = default)
    {
        if (NetworkManager.Singleton.ConnectedClients.ContainsKey(serverRpcParams.Receive.SenderClientId))
        {
            var client = NetworkManager.Singleton.ConnectedClients[serverRpcParams.Receive.SenderClientId];
            // Proceed with logic
        }
        else
        {
            Debug.LogError($"Client ID {serverRpcParams.Receive.SenderClientId} does not exist in ConnectedClients.");
        }

        Debug.Log("senderclientid: " + serverRpcParams.Receive.SenderClientId);
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;

        SendClientPosition(clientId, senderClientId);
    }
    private void SendClientPosition(ulong targetClientId, ulong senderClientId)
    {
        foreach (var client in NetworkManager.Singleton.ConnectedClients)
        {
            if (client.Key == targetClientId)
            {
                var targetClient = client.Value.PlayerObject;
                RecieveClientPositionClientRpc(targetClient.transform.position, targetClient.transform.rotation, senderClientId);
            }
        }

        Debug.LogError("No player object from client id found");
        
    }

    [ClientRpc]
    private void RecieveClientPositionClientRpc(Vector3 position, Quaternion rotation, ulong updatedClient)
    {
        if (NetworkManager.Singleton.LocalClientId == updatedClient)
        {
            targetPosition = position;
            targetRotation = rotation;
        }
    }
}
