using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KOTGFling : MonoBehaviour
{
    public float launchForce = 500f;
    private bool hit = false;
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (hit == false)
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Generate a random direction
                    Vector3 randomDirection = Random.onUnitSphere;

                    // Apply the launch force in the random direction
                    rb.AddForce(randomDirection * launchForce, ForceMode.Impulse);
                }
                else
                {
                    Debug.LogError("Rigidbody component missing from the player GameObject.");
                }
            }
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
