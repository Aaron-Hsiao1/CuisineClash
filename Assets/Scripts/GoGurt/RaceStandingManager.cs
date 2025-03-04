using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Linq;

public class RaceStandingsManager : NetworkBehaviour
{
    public List<GGStanding> racers = new List<GGStanding>();
    [SerializeField] private List<RawImage> placesImage;
    private Dictionary<ulong, int> playerPositions = new Dictionary<ulong, int>();

    private float updateInterval = 0.2f;
    private float lastUpdateTime;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Log detailed network spawn information
        Debug.Log($"RaceStandingsManager NetworkSpawn Details:");
        Debug.Log($"IsServer: {IsServer}");
        Debug.Log($"IsHost: {IsHost}");
        Debug.Log($"IsClient: {IsClient}");
        Debug.Log($"Local Client ID: {NetworkManager.Singleton.LocalClientId}");

        // Log place images
        if (placesImage == null)
        {
            Debug.LogError("CRITICAL: placesImage list is NULL!");
        }
        else
        {
            Debug.Log($"Place Images Count: {placesImage.Count}");
            for (int i = 0; i < placesImage.Count; i++)
            {
                if (placesImage[i] != null)
                {
                    Debug.Log($"Place Image {i}: {placesImage[i].gameObject.name} - Active: {placesImage[i].gameObject.activeInHierarchy}");
                }
                else
                {
                    Debug.LogWarning($"Place Image {i} is NULL!");
                }
            }
        }
    }

    void Start()
    {
        Debug.Log($"RaceStandingsManager Start - IsServer: {IsServer}, IsHost: {IsHost}, IsClient: {IsClient}");

        // Comprehensive reset of place images
        ResetPlaceImages();
    }

    void ResetPlaceImages()
    {
        if (placesImage == null)
        {
            Debug.LogError("Cannot reset place images - list is NULL!");
            return;
        }

        Debug.Log($"Resetting {placesImage.Count} place images");

        for (int i = 0; i < placesImage.Count; i++)
        {
            if (placesImage[i] != null)
            {
                // Use both methods to ensure visibility
                placesImage[i].enabled = false;
                placesImage[i].gameObject.SetActive(false);

                Debug.Log($"Reset place image {i}: {placesImage[i].gameObject.name}");
            }
            else
            {
                Debug.LogWarning($"Place image at index {i} is NULL!");
            }
        }
    }

    void Update()
    {
        // Only update periodically when on server or host
        if ((IsServer || IsHost) && Time.time - lastUpdateTime > updateInterval)
        {
            UpdateStandingsServer();
            lastUpdateTime = Time.time;
        }
    }

    private void UpdateStandingsServer()
    {
        // Find racers if list is empty
        if (racers.Count == 0)
        {
            FindAndAddRacers();
        }

        // Remove any null or invalid racers
        racers.RemoveAll(r => r == null || !r.IsSpawned);

        if (racers.Count == 0)
        {
            Debug.LogWarning("No racers found after filtering!");
            return;
        }

        Debug.Log($"Updating standings for {racers.Count} racers");

        // Safely sort racers, handling potential null or uninitialized progress
        var validRacers = racers
            .Where(r => r != null && r.progress != null)
            .OrderByDescending(r => r.progress.Value)
            .ToList();

        Debug.Log($"Valid racers after sorting: {validRacers.Count}");

        List<ulong> playerIds = new List<ulong>();
        List<int> ranks = new List<int>();

        for (int i = 0; i < validRacers.Count; i++)
        {
            GGStanding racer = validRacers[i];
            int newRank = i + 1;

            Debug.Log($"Racer {racer.OwnerClientId} - Progress: {racer.progress.Value}, Rank: {newRank}");

            playerIds.Add(racer.OwnerClientId);
            ranks.Add(newRank);
        }

        // Update clients with current standings
        UpdateStandingsClientRpc(playerIds.ToArray(), ranks.ToArray());
    }

    void FindAndAddRacers()
    {
        racers.Clear();
        var foundRacers = FindObjectsOfType<GGStanding>();

        Debug.Log($"Found {foundRacers.Length} potential racers in the scene");

        foreach (var racer in foundRacers)
        {
            if (racer.IsSpawned)
            {
                racers.Add(racer);
                Debug.Log($"Added racer: {racer.name}, Owner: {racer.OwnerClientId}");
            }
        }
    }

    [ClientRpc]
    private void UpdateStandingsClientRpc(ulong[] playerIds, int[] ranks)
    {
        Debug.Log($"UpdateStandingsClientRpc called - Local Client ID: {NetworkManager.Singleton.LocalClientId}");

        // Clear previous positions
        playerPositions.Clear();

        // Rebuild player positions
        for (int i = 0; i < playerIds.Length; i++)
        {
            playerPositions[playerIds[i]] = ranks[i];
            Debug.Log($"Player {playerIds[i]} ranked {ranks[i]}");
        }

        // Update place images for local client
        UpdateLocalPlayerPlaceImage();
    }

    private void UpdateLocalPlayerPlaceImage()
    {
        Debug.Log("Updating local player place image");

        // Reset all images first
        ResetPlaceImages();

        // Get local client ID
        ulong localClientId = NetworkManager.Singleton.LocalClientId;
        Debug.Log($"Local Client ID: {localClientId}");

        // Check if local client has a position
        if (playerPositions.TryGetValue(localClientId, out int rank))
        {
            Debug.Log($"Local client rank: {rank}");

            // Ensure rank is valid and within image list
            if (rank > 0 && rank <= placesImage.Count)
            {
                // Enable the corresponding place image
                if (placesImage[rank - 1] != null)
                {
                    placesImage[rank - 1].enabled = true;
                    placesImage[rank - 1].gameObject.SetActive(true);
                    Debug.Log($"Enabled place image for rank {rank}");
                }
                else
                {
                    Debug.LogError($"Place image for rank {rank} is NULL!");
                }
            }
            else
            {
                Debug.LogWarning($"Rank {rank} is out of bounds. Max places: {placesImage.Count}");
            }
        }
        else
        {
            Debug.LogWarning($"No position found for local client {localClientId}");
        }
    }
}