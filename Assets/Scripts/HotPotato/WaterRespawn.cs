using UnityEngine;

public class WaterRespawn : MonoBehaviour
{
    // Reference to the spawn point (set this in the Inspector)
    public Transform spawnPoint;

    // Check if the player collides with the table
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object that collided with the table has the "Player" tag
        if (collision.gameObject.CompareTag("Player"))
        {
            // Teleport the player to the spawn point
            collision.gameObject.transform.position = spawnPoint.position;
        }
    }
}