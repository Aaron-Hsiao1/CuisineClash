//this component should be added to each Racer
//it requires a Rigidbody and a collider

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGLapCounter : MonoBehaviour
{

    private void OnEnable()
    {
        GGLapManager.instance.AddRacerToProgressKeeper(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Do something when we hit a checkpoint
        if (other.GetComponent<Checkpoint>())
        {
            GGLapManager.instance.RacerProgressReport(this, other.GetComponent<Checkpoint>());
        }
        else
        {
            Debug.Log($" {other.name} does not have a checkpoint component");
        }

    }
}

