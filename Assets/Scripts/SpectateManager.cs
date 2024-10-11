using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class SpectateManager : NetworkBehaviour
{
    public SpectateManager Instance { get; private set; }
    
    private List<ulong> alivePlayers;

    private Dictionary<ulong, NetworkClient> connectedClients;

    [SerializeField] private Camera spectatorCamera;
    private void Awake()
    {
        Instance = this;
        
        alivePlayers = new List<ulong>();

        Debug.Log("is rainingmeatballmanager nulLL: " + RainingMeatballManager.Instance == null);
        
    }
    public override void OnNetworkSpawn()
    {
        Debug.Log("SpectateManager is running on server. Starting event subscription retry.");
        StartCoroutine(SubscribeToDeathZoneEvent());

        if (IsHost)
        {
            Debug.Log("server");
            foreach (var player in NetworkManager.ConnectedClientsIds)
            {
                Debug.Log("for loop");
                var playerData = CuisineClashMultiplayer.Instance.GetPlayerDataFromClientId(player);

                if (playerData.isAlive == true)
                {
                    alivePlayers.Add(player);
                    Debug.Log("player added to alive players " + playerData.playerName.ToString());
                }
            }
        }

    }

    private IEnumerator SubscribeToDeathZoneEvent()
    {
        while (DeathZone.Instance == null)
        {
            Debug.Log("Waiting for DeathZone instance to be initialized...");
            yield return new WaitForSeconds(0.5f); // Retry every half second
        }

        Debug.Log("DeathZone instance found. Subscribing to OnAlivePlayersChanged.");
        DeathZone.Instance.OnAlivePlayersChanged += SpectateManager_OnAlivePlayersChanged;
    }

    private void SpectateManager_OnAlivePlayersChanged(object sender, EventArgs e)
    {
        if (IsHost)
        {
            Debug.Log("alive playertschanged 2");
            alivePlayers.Clear();
            foreach (var player in NetworkManager.ConnectedClientsIds)
            {
                var playerData = CuisineClashMultiplayer.Instance.GetPlayerDataFromClientId(player);

                if (playerData.isAlive == true)
                {
                    alivePlayers.Add(player);
                    Debug.Log("player added: " + playerData.playerName);
                }

            }
        }
        
    }

    private ulong GetFirstSpectatablePlayer()
    {
        if (alivePlayers.Count > 0)
        {
            return alivePlayers[0];
        }
        else
        {
            Debug.LogWarning("alivePlayers.Count == 0");
            return 0;
        }
        
    }

    public void BecomeSpectator(ulong clientId)
    {
        BecomeSpectatorServerRpc(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void BecomeSpectatorServerRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient networkClient))
        {
            GameObject playerObject = networkClient.PlayerObject.gameObject;

            playerObject.GetComponentInChildren<Camera>().gameObject.SetActive(false);

            ulong targetClientId = GetFirstSpectatablePlayer();

            SpawnSpectatorCameraForPlayerClientRpc(clientId, targetClientId);
            Debug.Log("you are now a spectator"); //make the spectator's camera = to the spectated player
        }
    }

    [ClientRpc]
    public void SpawnSpectatorCameraForPlayerClientRpc(ulong clientId, ulong targetClientId)
    {
        SpawnSpectatorCameraForPlayer(clientId, targetClientId);
    }

    public void SpawnSpectatorCameraForPlayer(ulong spectatorClientId, ulong targetClientId)
    {
        if (NetworkManager.Singleton.LocalClientId == spectatorClientId)
        {
            RequestTargetCameraDataServerRpc(targetClientId, spectatorClientId);

            /*var camera = Instantiate(spectatorCamera, new Vector3(0, 10, 0), Quaternion.identity);

            var targetPlayerObject = ReturnPlayerObjectServerRpc(clientId)[targetClientId];

            Camera targetCamera = targetPlayerObject.transform.Find("CameraHolder").Find("Main Camera").GetComponent<Camera>();

            Debug.Log("camera transform: " + camera.transform.position);
            Debug.Log("target camera transform: " + targetCamera.transform.position);


            camera.transform.position = new Vector3(0, 0, 0);
            camera.transform.rotation = targetPlayerObject.transform.rotation;

            camera.GetComponent<SpectatorCamera>().SetTargetCamera(targetCamera);*/
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestTargetCameraDataServerRpc(ulong targetClientId, ulong spectatorClientId)
    {
        RequestTargetCameraData(targetClientId, spectatorClientId);
    }

    public void RequestTargetCameraData(ulong targetClientId, ulong spectatorClientId)
    {
        var targetPlayerObject = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject;

        if (targetPlayerObject != null)
        {
            // Find the camera component on the target player object
            Camera targetCamera = targetPlayerObject.transform.Find("CameraHolder").Find("Main Camera").GetComponent<Camera>();

            if (targetCamera != null)
            {
                // Send the target camera's position and rotation back to the client
                SendTargetCameraDataClientRpc(targetClientId, spectatorClientId, targetCamera.transform.position, targetCamera.transform.rotation);
            }
        }
    }

    [ClientRpc]
    private void SendTargetCameraDataClientRpc(ulong clientId, ulong spectatorClientId, Vector3 cameraPosition, Quaternion cameraRotation)
    {
        SendTargetCameraData(clientId, spectatorClientId, cameraPosition, cameraRotation);
    }

    private void SendTargetCameraData(ulong clientId, ulong spectatorClientId, Vector3 cameraPosition, Quaternion cameraRotation)
    {
        if (NetworkManager.Singleton.LocalClientId == spectatorClientId)
        {
            var spectatorCam = Instantiate(spectatorCamera, cameraPosition, cameraRotation);

            spectatorCam.transform.position = cameraPosition;
            spectatorCam.transform.rotation = cameraRotation;



            spectatorCam.GetComponent<SpectatorCamera>().SetCameraPositionAndRotation(cameraPosition, cameraRotation); //works but need to update continuously

            Debug.Log("Spectator camera spawned at position: " + cameraPosition);

        }
    }


    

    // Update is called once per frame
    void Update()
    {
        
    }
}

