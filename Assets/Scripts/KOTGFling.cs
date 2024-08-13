using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KOTGFling : MonoBehaviour
{
    public float horizontalLaunchForce = 10f;
    public float launchForceUp = 100f;
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
                
                    rb.AddForce(new Vector3(randomDirection.x * horizontalLaunchForce, launchForceUp, randomDirection.z * horizontalLaunchForce), ForceMode.Impulse);
                    hit = true;
                    Debug.Log("hit");
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
        float angle = Random.Range(0, 360);

        Vector3 direction = new Vector3(Mathf.Cos(angle * (Mathf.PI / 180)), 0, Mathf.Sin(angle * (Mathf.PI / 180)));

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
