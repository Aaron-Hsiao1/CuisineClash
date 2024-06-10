using System.Collections;
using UnityEngine;

public class MeatballSpawner : MonoBehaviour
{
    public GameObject meatballPrefab; // The meatball prefab
    public float spawnAreaWidth = 10f; // Width of the spawn area
    public float spawnAreaLength = 10f; // Length of the spawn area
    public float spawnHeight = 20f; // Height at which meatballs will spawn
    public float spawnInterval = 1f; // Time interval between spawns

    void Start()
    {
        // Start the coroutine to spawn meatballs at regular intervals
        StartCoroutine(SpawnMeatballs());
    }

    IEnumerator SpawnMeatballs()
    {
        while (true)
        {
            // Wait for the specified interval
            yield return new WaitForSeconds(spawnInterval);

            // Calculate a random position within the spawn area
            Vector3 spawnPosition = new Vector3(
                Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2),
                spawnHeight,
                Random.Range(-spawnAreaLength / 2, spawnAreaLength / 2)
            );

            // Instantiate the meatball at the random position
            GameObject meatball = Instantiate(meatballPrefab, spawnPosition, Quaternion.identity);

            // Ensure the meatball has a Rigidbody component
            Rigidbody rb = meatball.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = meatball.AddComponent<Rigidbody>();
            }

            // Ensure gravity is enabled
            rb.useGravity = true;
        }
    }
}
