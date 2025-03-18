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


        // Log place images
        if (placesImage == null)
        {
        }
        else
        {
            Debug.Log($"Place Images Count: {placesImage.Count}");
            for (int i = 0; i < placesImage.Count; i++)
            {
                if (placesImage[i] != null)
                {
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

        // Comprehensive reset of place images
        ResetPlaceImages();
    }

    void ResetPlaceImages()
    {
        if (placesImage == null)
        {
            return;
        }

        for (int i = 0; i < placesImage.Count; i++)
        {
            if (placesImage[i] != null)
            {
                // Use both methods to ensure visibility
                placesImage[i].enabled = false;
                placesImage[i].gameObject.SetActive(false);

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

        // Safely sort racers, handling potential null or uninitialized progress
        var validRacers = racers
            .Where(r => r != null && r.progress != null)
            .OrderByDescending(r => r.progress.Value)
            .ToList();

        List<ulong> playerIds = new List<ulong>();
        List<int> ranks = new List<int>();

        for (int i = 0; i < validRacers.Count; i++)
        {
            GGStanding racer = validRacers[i];
            int newRank = i + 1;


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

        foreach (var racer in foundRacers)
        {
            if (racer.IsSpawned)
            {
                racers.Add(racer);
            }
        }
    }

    [ClientRpc]
    private void UpdateStandingsClientRpc(ulong[] playerIds, int[] ranks)
    {

        // Clear previous positions
        playerPositions.Clear();

        // Rebuild player positions
        for (int i = 0; i < playerIds.Length; i++)
        {
            playerPositions[playerIds[i]] = ranks[i];
        }

        // Update place images for local client
        UpdateLocalPlayerPlaceImage();
    }

    private void UpdateLocalPlayerPlaceImage()
    {

        // Reset all images first
        ResetPlaceImages();

        // Get local client ID
        ulong localClientId = NetworkManager.Singleton.LocalClientId;

        // Check if local client has a position
        if (playerPositions.TryGetValue(localClientId, out int rank))
        {

            // Ensure rank is valid and within image list
            if (rank > 0 && rank <= placesImage.Count)
            {
                // Enable the corresponding place image
                if (placesImage[rank - 1] != null)
                {
                    placesImage[rank - 1].enabled = true;
                    placesImage[rank - 1].gameObject.SetActive(true);
                }
                else
                {
                }
            }
            else
            {
            }
        }
        else
        {
        }
    }
}