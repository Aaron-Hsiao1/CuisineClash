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

    public void SpawnSpectatorCameraForPlayer(ulong clientId, ulong targetClientId)
    {
        var camera = Instantiate(spectatorCamera, new Vector3(0, 10, 0), Quaternion.identity);
        
        var targetPlayerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

        var targetPlayerObjectTransform = targetPlayerObject.transform.GetChild(0).GetChild(0).transform;

        camera.transform.position = targetPlayerObjectTransform.position;
        camera.transform.rotation = targetPlayerObjectTransform.rotation;

        if (NetworkManager.Singleton.LocalClientId != clientId) {
            camera.gameObject.SetActive(false);
        }


    }

    [ClientRpc]
    public void SpawnSpectatorCameraForPlayerClientRpc(ulong clientId, ulong targetClientId)
    {
        SpawnSpectatorCameraForPlayer(clientId, targetClientId);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
