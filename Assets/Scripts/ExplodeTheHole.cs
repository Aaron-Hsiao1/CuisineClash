using UnityEngine;
using System.Collections;

public class Indicator : MonoBehaviour
{
    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.CompareTag("Meatball"))
        {
            Debug.Log("Stinky");
            Destroy(gameObject);
        }
    }
}
