using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaceStandingsManager : MonoBehaviour
{
    public List<GGStanding> racers = new List<GGStanding>(); // List of all racers
    [SerializeField] private TextMeshProUGUI playerPositionText; // UI Text to display player's position

    void Update()
    {
        UpdateStandings();
    }

    void UpdateStandings()
    {
        // Sort racers by their progress (higher progress means ahead in the race)
        racers.Sort((r1, r2) => r2.progress.CompareTo(r1.progress));

        for (int i = 0; i < racers.Count; i++)
        {
            racers[i].currentRank = i + 1;
        }

        // Update the player's position UI (assuming the first racer is the player)
        playerPositionText.text = "" + GetOrdinalSuffix(racers[0].currentRank);
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
