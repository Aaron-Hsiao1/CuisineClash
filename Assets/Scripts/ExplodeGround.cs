using UnityEngine;
using System.Collections;

public class Meatball : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        // Check if the meatball collided with the ground plane
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Destroy the meatball when it touches the ground
            Destroy(gameObject);
        }
    }
}
