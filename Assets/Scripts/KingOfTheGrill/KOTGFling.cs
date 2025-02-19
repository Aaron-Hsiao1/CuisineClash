using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KOTGFling : MonoBehaviour
{
    public float horizontalLaunchForce = 500000f; // Stronger horizontal push
    public float launchForceUp = 10f; // Reduced vertical force
    private bool hit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hit)
        {
            Rigidbody rb = other.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDirection = GetRandomHorizontalDirection();

                // Apply mostly horizontal force with a slight upward push
                Vector3 totalForce = (randomDirection * horizontalLaunchForce) + (Vector3.up * launchForceUp);
                rb.AddForce(totalForce, ForceMode.Impulse);

                hit = true;
            }
        }
    }

    Vector3 GetRandomHorizontalDirection()
    {
        Vector2 randomXZ = Random.insideUnitCircle.normalized; // Get a random 2D direction
        return new Vector3(randomXZ.x, 0, randomXZ.y); // Convert to 3D
    }
}
