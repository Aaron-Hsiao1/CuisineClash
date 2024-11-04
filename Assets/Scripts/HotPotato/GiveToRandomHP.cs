using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GiveToRandomHP : NetworkBehaviour
{
    public float cooldownTime = 5.0f; // Cooldown time before assigning the hot potato to a new player

    private List<ulong> playerIDs;     // List of player IDs
    private ulong currentPlayerWithPotato; // ID of the player currently holding the potato

    void Start()
    {
        // Ensure we assign the potato only on the server
        if (IsServer)
        {
            playerIDs = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);
            if (playerIDs.Count > 1)
            {
                AssignRandomPlayerWithPotato();
            }
        }
    }

    // Reassigns the hot potato to a random player, excluding the current holder
    private void AssignRandomPlayerWithPotato()
    {
        if (playerIDs.Count < 2) return; // Ensure there's more than one player available

        // Create a list of possible targets by excluding the current player holding the potato
        List<ulong> possibleTargets = new List<ulong>(playerIDs);
        possibleTargets.Remove(currentPlayerWithPotato);

        // Randomly select a new player from the possible targets
        currentPlayerWithPotato = possibleTargets[UnityEngine.Random.Range(0, possibleTargets.Count)];

        Debug.Log("New player with hot potato: " + currentPlayerWithPotato);

        // Inform all clients who now has the potato
        SetHotPotatoClientRpc(currentPlayerWithPotato);
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

    // This ClientRpc syncs the hot potato state across clients
    [ClientRpc]
    public void SetHotPotatoClientRpc(ulong potatoHolderId)
    {
        foreach (var player in FindObjectsOfType<HotPotatoTag>())
        {
            // Update hasHotPotato status
            player.hasHotPotato = (player.OwnerClientId == potatoHolderId);

            // Update visibility immediately
            player.UpdateHotPotatoVisibility();

            if (player.hasHotPotato)
            {
                Debug.Log($"{player.gameObject.name} now has the hot potato.");
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        // This logic ensures that playerIDs are only populated once when the network is spawned
        if (IsServer)
        {
            playerIDs = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);

            // Assign the hot potato to a random player at the start if enough players
            if (playerIDs.Count > 1)
            {
                AssignRandomPlayerWithPotato();
            }
        }
    }
}
