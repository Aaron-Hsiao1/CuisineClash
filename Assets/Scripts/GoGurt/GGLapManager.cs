


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGLapManager : MonoBehaviour
{
    public static GGLapManager instance;

    [Header("Drag and drop your checkpoints here - in their correct order")]
    [SerializeField] private List<Checkpoint> checkpoints;


    //here (int, int) is a tuple the first item is the Lap, the second is the current/mostRecent checkpoint which was hit
    //if you like you could make a struct instead
    Dictionary<GGLapCounter, (int, int)> racerProgress;

    private void OnEnable()
    {
        instance = this;
        racerProgress = new Dictionary<GGLapCounter, (int, int)>();
    }

    public void RacerProgressReport(GGLapCounter r, Checkpoint c)
    {
        //index of might not be great, each checkpoint could be given it's idex to hold or something
        var hitCheckpoint = checkpoints.IndexOf(c);

        //get the current/most recent checkpoint hit by racer
        var currentRacerCP = racerProgress[r].Item2;


        if (hitCheckpoint == 0 && currentRacerCP == checkpoints.Count - 1)  //this checks if we have hit the start/end but it also assumes that start and end will be in the same place
        {
            racerProgress[r] = (racerProgress[r].Item1 + 1, 0);
            Debug.Log($"We done did a lap! we're now on {racerProgress[r].Item1}");

        }
        else if (hitCheckpoint == currentRacerCP + 1) //currentRacerCP + 1 is the next one we should hit in sequence
        {
            racerProgress[r] = (racerProgress[r].Item1, racerProgress[r].Item2 + 1);
            Debug.Log($"racer {r} has hit {c}!");
        }
        else if (hitCheckpoint <= currentRacerCP)
        {
            Debug.Log($"racer {r} has hit {c}! YOU'RE GOING BACKWARDS");
        }
        else
        {
            Debug.Log($"racer is at: lap {racerProgress[r].Item1} checkpoint {racerProgress[r].Item2} and hit {checkpoints.IndexOf(c)}, CHEATER!!!");
        }
    }

    public void AddRacerToProgressKeeper(GGLapCounter r)
    {
        //might want to do some business here to make sure everything is propperly instantiated.
        racerProgress.Add(r, (0, -1));
    }


    //these are probably
    public void AddCheckpointToList(Checkpoint c)
    {
        //again, business to make sure everything is correct should probably be done here in production code
        checkpoints.Add(c);
    }

    public void RemoveCheckpointFromList(Checkpoint c)
    {
        //this might not be necessary ever
        checkpoints.Remove(c);
    }

}
