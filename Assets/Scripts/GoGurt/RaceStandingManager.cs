using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class RaceStandingsManager : NetworkBehaviour
{
    public List<GGStanding> racers = new List<GGStanding>(); 
    [SerializeField] private TextMeshProUGUI playerPositionText;

    void Update()
    {
        if (IsServer)
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
        racers.Sort((r1, r2) => r2.progress.Value.CompareTo(r1.progress.Value));

        for (int i = 0; i < racers.Count; i++)
        {
            racers[i].currentRank = i + 1;
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
            playerPositionText.text = GetOrdinalSuffix(localPlayer.currentRank);
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
