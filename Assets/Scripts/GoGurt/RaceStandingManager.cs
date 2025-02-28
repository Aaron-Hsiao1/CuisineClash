using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class RaceStandingsManager : NetworkBehaviour
{
    public List<GGStanding> racers = new List<GGStanding>();
    [SerializeField] List<RawImage> placesImage;
    private Dictionary<ulong, int> playerPositions = new Dictionary<ulong, int>();
    
    void Start()
    {
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

        racers.Sort((r1, r2) => r2.progress.Value.CompareTo(r1.progress.Value));

        List<ulong> playerIds = new List<ulong>();
        List<int> ranks = new List<int>();

        for (int i = 0; i < racers.Count; i++)
        {
            GGStanding racer = racers[i];
            int newRank = i + 1;
            if (racer.currentRank != newRank)
            {
                racer.currentRank = newRank;
            }
            playerIds.Add(racer.OwnerClientId);
            ranks.Add(newRank);
        }

        // Convert to arrays before sending
        UpdateStandingsClientRpc(playerIds.ToArray(), ranks.ToArray());
    }



    [ClientRpc]
    private void UpdateStandingsClientRpc(ulong[] playerIds, int[] ranks)
    {
        // Convert back into a dictionary
        playerPositions.Clear();
        for (int i = 0; i < playerIds.Length; i++)
        {
            playerPositions[playerIds[i]] = ranks[i];
        }

        UpdatePlaceImages();
    }



    private void UpdatePlaceImages()
    {
        foreach (var image in placesImage)
        {
            image.enabled = false;
        }
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