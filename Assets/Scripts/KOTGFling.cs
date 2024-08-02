using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KOTGFling : MonoBehaviour
{
    public float launchForce = 0f;
    public float launchForceUp = 10f;
    private bool hit = false;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (hit == false)
            {
                Rigidbody rb = other.GetComponentInParent<Rigidbody>();
                if (rb != null)
                {
                    // Generate a random direction
                    Vector3 randomDirection = GetRandomHorizontalDirection();

                    // Apply the launch force in the random direction
                    rb.AddForce(randomDirection * launchForce, ForceMode.Impulse);
                    rb.AddForce(Vector3.up * launchForceUp, ForceMode.Impulse);
                    hit = true;
                    Debug.LogError("hit");
                }
                else
                {
                    Debug.LogError("Rigidbody component missing from the player GameObject.");
                }
            }
        }
    }
    Vector3 GetRandomHorizontalDirection()
    {
        // Generate a random angle in radians
        float angle = Random.Range(0f, 2f * Mathf.PI);

        // Calculate the direction based on the angle
        Vector3 direction = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle));

        return direction.normalized; // Normalize to ensure consistent force magnitude
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
