using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GGSpeedUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider trigger)
    {
        Debug.Log("Trigger entered");
        if (trigger.gameObject.CompareTag("Player"))
        {
            Debug.Log("Boost activated");
            trigger.gameObject.GetComponent<GoKartMovement>().CurrentSpeed = +500f;
        }
        //StartCoroutine(BoostTimer());
    }

    IEnumerator BoostTimer()
    {
        yield return new WaitForSeconds(5f);
        gameObject.GetComponent<GGSpeedUp>().enabled = false;

    }
}
