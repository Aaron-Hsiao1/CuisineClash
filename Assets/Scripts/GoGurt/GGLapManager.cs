using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class GGLapManager : MonoBehaviour
{
    public static GGLapManager instance;

    [Header("Drag and drop your checkpoints here - in their correct order")]
    [SerializeField] private List<Checkpoint> checkpoints;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI lapText;
    [SerializeField] private RawImage FinishText;
    [SerializeField] private TextMeshProUGUI raceStatusText; // Text for displaying race status
    private bool IsFinished = false; 

    [Header("Race Settings")]
    [SerializeField] private int maxLaps = 3;
    private int PlayersFinished = 0;
    [SerializeField] private int PlayersNeededtoFinish = 3;

    private Dictionary<GGLapCounter, (int, int)> racerProgress;

    private void Awake()
    {
        FinishText.enabled = false;
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
                GoKartMovement.canMove = false;
                FinishText.enabled = true;
                //PlayersFinished = +1;
                //if (PlayersFinished >= PlayersNeededtoFinish)
                //{
                //StartCoroutine(ShowEndGameUIs());
                //}
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

    IEnumerator ShowEndGameUIs()
    {
        Cursor.lockState = CursorLockMode.None;
        FinishText.enabled = true; 
        yield return new WaitForSeconds(5f);
        FinishText.enabled = false;

        yield return new WaitForSeconds(3f);

        if (GamemodeManager.Instance.GetGamemodeList().Count > 0)
        {
            Loader.LoadNetwork(Loader.Scene.PregameLobby.ToString());
        }
        if (GamemodeManager.Instance.GetGamemodeList().Count == 0)
        {
            Loader.LoadNetwork(Loader.Scene.GameEnded.ToString());
        }
    }
}
