using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GiveToRandomHP : NetworkBehaviour
{
    public float cooldownTime = 5.0f; // Cooldown time before assigning the hot potato to a new player

    private List<ulong> alivePlayerIds;     // List of player IDs
    private ulong currentPlayerWithPotato; // ID of the player currently holding the potato

    [SerializeField] private GameObject playerWithPotatoPrefab;

    public override void OnNetworkSpawn()
    {
        // This logic ensures that playerIDs are only populated once when the network is spawned
        if (IsServer)
        {
            alivePlayerIds = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);

            // Assign the hot potato to a random player at the start if enough players
            if (alivePlayerIds.Count > 1)
            {
                AssignRandomPlayerWithPotato();
            }
        }
    }

    // Reassigns the hot potato to a random player, excluding the current holder
    private void AssignRandomPlayerWithPotato() //on server
    {
        if (alivePlayerIds.Count < 2)
        {
            Debug.Log("Less than 2 people alive");
            return; // Ensure there's more than one player available
        }

        int randomPlayer = Random.Range(0, alivePlayerIds.Count);
        currentPlayerWithPotato = alivePlayerIds[randomPlayer];

        Debug.Log("New player with hot potato: " + currentPlayerWithPotato);

        // Inform all clients who now has the potato
        SetHotPotatoClientRpc(currentPlayerWithPotato);
    }

    // This ClientRpc syncs the hot potato state across clients
    [ClientRpc]
    public void SetHotPotatoClientRpc(ulong potatoHolderId)
    {
        Debug.Log("Potato holder id: " + potatoHolderId);
        Debug.Log("is networkmanager null" + NetworkManager.Singleton);
        //playerWithPotatoPrefab = CuisineClashMultiplayer.Instance.GetPlayerObjectFromPlayerId(potatoHolderId).transform.Find("PlayerObj/CHACTER1animationattempt/temppotato").gameObject;
        Debug.Log("is potato player prefab null: " + playerWithPotatoPrefab == null);
        Debug.Log("potato set active");
        //playerWithPotatoPrefab.SetActive(true);
        if (NetworkManager.Singleton.LocalClientId == potatoHolderId)
        {
            Debug.Log("if statemetn ran");
            playerWithPotatoPrefab = NetworkManager.Singleton.LocalClient.PlayerObject.transform.Find("PlayerObj/CHACTER1animationattempt/temppotato").gameObject;
            Debug.Log("playerpotatoprefab set");
            playerWithPotatoPrefab.SetActive(true);
        }  
        
    }

    [ServerRpc]
    public void SetHotPotatoClientRpc(ulong potatoHolderId) //TRY AND GET THE SERVER TO RUNT HE CODE, MAYBE SET POTATO TO ALWAYS BE THE HO ST WHEN TESTING
    {
        Debug.Log("Potato holder id: " + potatoHolderId);
        Debug.Log("is networkmanager null" + NetworkManager.Singleton);
        //playerWithPotatoPrefab = CuisineClashMultiplayer.Instance.GetPlayerObjectFromPlayerId(potatoHolderId).transform.Find("PlayerObj/CHACTER1animationattempt/temppotato").gameObject;
        Debug.Log("is potato player prefab null: " + playerWithPotatoPrefab == null);
        Debug.Log("potato set active");
        //playerWithPotatoPrefab.SetActive(true);
        if (NetworkManager.Singleton.LocalClientId == potatoHolderId)
        {
            Debug.Log("if statemetn ran");
            playerWithPotatoPrefab = NetworkManager.Singleton.LocalClient.PlayerObject.transform.Find("PlayerObj/CHACTER1animationattempt/temppotato").gameObject;
            Debug.Log("playerpotatoprefab set");
            playerWithPotatoPrefab.SetActive(true);
        }

    }

    [ServerRpc]
    public void SetHotPotatoServerRpc(ulong potatohHolderId)
    {

    }

    // Call this function when the potato explodes
    public void OnPotatoExploded()
    {
        StartCoroutine(ReassignPotatoAfterCooldown());
    }

    private IEnumerator ReassignPotatoAfterCooldown()
    {
        yield return new WaitForSeconds(cooldownTime); // Wait for the cooldown time
        AssignRandomPlayerWithPotato(); // Reassign to a random player
    }

    

    
}
