using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private bool playerInArea = false;
    private float timer = 0f;
    private float delay = 1f;

    // Function called when another collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the entering collider has the "Player" tag
        {
            playerInArea = true;

        }
    }

    // Function called when another collider exits the trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the exiting collider has the "Player" tag
        {
            playerInArea = false;
            timer = 0f;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInArea)
        {
            timer += Time.deltaTime;
            if (timer >= delay)
            {
                ScoreManager.instance.AddPoint();
                timer = 0f; // Reset the timer
            }
        }
    }
}
