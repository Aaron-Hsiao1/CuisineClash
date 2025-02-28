using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class RaceStandingsManager : NetworkBehaviour
{
    public List<GGStanding> racers = new List<GGStanding>();
    [SerializeField] List<RawImage> placesImage;
    
    // Dictionary to track player positions
    private Dictionary<ulong, int> playerPositions = new Dictionary<ulong, int>();
    
    void Start()
    {
        // Disable all place images initially
        foreach (var image in placesImage)
        {
            image.enabled = false;
        }
    }
    
    void Update()
    {
        if (IsServer || IsHost)
        {
            UpdateStandingsServer();
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    void RequestStandingsUpdateServerRpc()
    {
        UpdateStandingsServer();
    }
    
    private void UpdateStandingsServer()
    {
        if (racers.Count == 0)
        {
            Debug.Log("No racers found");
            return;
        }
        
        // Sort racers by progress (highest first)
        racers.Sort((r1, r2) => r2.progress.Value.CompareTo(r1.progress.Value));
        
        // Update ranks for all racers
        for (int i = 0; i < racers.Count; i++)
        {
            GGStanding racer = racers[i];
            int newRank = i + 1;
            
            // Only update if rank changed
            if (racer.currentRank != newRank)
            {
                racer.currentRank = newRank;
                playerPositions[racer.OwnerClientId] = newRank;
            }
        }
        
        // Send updated standings to all clients
        UpdateStandingsClientRpc(playerPositions);
    }
    
    [ClientRpc]
    private void UpdateStandingsClientRpc(Dictionary<ulong, int> positions)
    {
        // Update place images for the local player
        UpdatePlaceImages();
    }
    
    private void UpdatePlaceImages()
    {
        // First disable all images
        foreach (var image in placesImage)
        {
            image.enabled = false;
        }
        
        // Enable only the relevant image based on local player's position
        foreach (var racer in racers)
        {
            if (racer.OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                int rank = racer.currentRank;
                if (rank > 0 && rank <= placesImage.Count)
                {
                    placesImage[rank - 1].enabled = true;
                }
                break;
            }
        }
    }
}