using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GGLapManager : MonoBehaviour
{
    public static GGLapManager instance;

    [Header("Drag and drop your checkpoints here - in their correct order")]
    [SerializeField] private List<Checkpoint> checkpoints;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private TextMeshProUGUI raceStatusText; // Text for displaying race status

    [Header("Race Settings")]
    [SerializeField] private int maxLaps = 3; // Max laps to complete the race

    private Dictionary<GGLapCounter, (int, int)> racerProgress;

    private void Awake()
    {
        instance = this;
        racerProgress = new Dictionary<GGLapCounter, (int, int)>();
    }

    public void RacerProgressReport(GGLapCounter r, Checkpoint c)
    {
        if (!racerProgress.ContainsKey(r))
        {
            AddRacerToProgressKeeper(r);
        }

        var hitCheckpoint = checkpoints.IndexOf(c);
        var currentRacerCP = racerProgress[r].Item2;

        if (hitCheckpoint == 0 && currentRacerCP == checkpoints.Count - 1)
        {
            int newLap = racerProgress[r].Item1 + 1;
            if (newLap >= maxLaps)
            {
                Debug.Log("Race Finished!");
                raceStatusText.text = "Race Finished";
                GoKartMovement.canMove = false; 
                return; 
            }

            racerProgress[r] = (newLap, 0);

            // Update UI
            if (lapText != null)
            {
                lapText.text = $"Lap: {newLap}/{maxLaps}";
            }
        }
        else if (hitCheckpoint == currentRacerCP + 1)
        {
            racerProgress[r] = (racerProgress[r].Item1, hitCheckpoint);
        }
    }

    public void AddRacerToProgressKeeper(GGLapCounter r)
    {
        if (!racerProgress.ContainsKey(r))
        {
            racerProgress.Add(r, (0, -1));
        }
    }
}
