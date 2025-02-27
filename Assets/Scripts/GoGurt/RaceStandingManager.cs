using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.UI;

public class RaceStandingsManager : NetworkBehaviour
{
    public List<GGStanding> racers = new List<GGStanding>(); 
    [SerializeField] private TextMeshProUGUI playerPositionText;
    [SerializeField] private Dictionary<int, ulong> playersAndplace;
    [SerializeField] List<RawImage> placesImage;
    private List<ulong> players;

    void Update()
    {
        if (IsHost)
        {
            UpdateStandingsServer();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestStandingsUpdateServerRpc()
    {
        Debug.Log("UpdateStandingServer");
        UpdateStandingsServer();
    }

    private void UpdateStandingsServer()
    {
        Debug.Log(racers);
        players = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);
        Debug.Log("The racer count is" + players.Count);
        racers.Sort((r1, r2) => r2.progress.Value.CompareTo(r1.progress.Value));
        Debug.Log("The racer count is" + racers.Count);
        for (int i = 0; i < racers.Count; i++)
        {
            racers[i].currentRank = i + 1;
            Debug.Log(racers[i].currentRank);
        }

        UpdateStandingsClientRpc();
    }

    [ClientRpc]
    private void UpdateStandingsClientRpc()
    {
        if (!IsOwner) return;

        GGStanding localPlayer = racers.Find(r => r.OwnerClientId == NetworkManager.Singleton.LocalClientId);
        if (localPlayer != null)
        {
            for (int i = 0; i < racers.Count; i++)
            {
                if (racers[i].currentRank == 1)
                {
                    placesImage[0].enabled = true;
                    placesImage[1].enabled = false;
                }
                else if (racers[i].currentRank == 2)
                {
                    placesImage[1].enabled = true;
                    placesImage[2].enabled = false;
                    placesImage[0].enabled = false;
                }
                else if (racers[i].currentRank == 2)
                {
                    placesImage[1].enabled = false;
                    placesImage[2].enabled = true;

                }
                else if (racers[i].currentRank == 2)
                {
                    placesImage[3].enabled = true;
                }
                else if (racers[i].currentRank == 2)
                {
                    placesImage[4].enabled = true;
                }
                else if (racers[i].currentRank == 2)
                {
                    placesImage[5].enabled = true;
                }
                else if (racers[i].currentRank == 2)
                {
                    placesImage[6].enabled = true;
                }
                else if (racers[i].currentRank == 2)
                {
                    placesImage[7].enabled = true;
                }

            }
        }
    }

    string GetOrdinalSuffix(int num)
    {
        if (num % 100 >= 11 && num % 100 <= 13) return num + "th";
        switch (num % 10)
        {
            case 1: return num + "st";
            case 2: return num + "nd";
            case 3: return num + "rd";
            default: return num + "th";
        }
    }
}
