using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpectateManager : NetworkBehaviour
{
    private List<ulong> alivePlayers;
    private void Awake()
    {
        alivePlayers = new List<ulong>();

        Debug.Log("is rainingmeatballmanager nulLL: " + RainingMeatballManager.Instance == null);
        
    }
    public override void OnNetworkSpawn()
    {
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

            DeathZone.Instance.OnAlivePlayersChanged += SpectateManager_OnAlivePlayersChanged;
        }

        
    }

    // Start is called before the first frame update
    void Start()
    {
        
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
