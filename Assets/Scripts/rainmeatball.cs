using System.Collections;
using UnityEngine;

public class MeatballSpawner : MonoBehaviour
{
    public GameObject meatballPrefab;
    public GameObject indicatorPrefab;
    public float spawnAreaWidth = 10f;
    public float spawnAreaLength = 10f;
    public float spawnHeight = 20f;
    public float initialSpawnInterval = 1f;
    public float speedUpInterval = 60f; 

    private float spawnInterval; 
    private float elapsedTime; 

    void Start()
    {
        spawnInterval = initialSpawnInterval;
        elapsedTime = 0f;
        StartCoroutine(SpawnMeatballs());
    }

    IEnumerator SpawnMeatballs()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            Vector3 spawnPosition = new Vector3(
                Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2),
                spawnHeight,
                Random.Range(-spawnAreaLength / 2, spawnAreaLength / 2)
            );

            GameObject meatball = Instantiate(meatballPrefab, spawnPosition, Quaternion.identity);
            GameObject indicator = Instantiate(indicatorPrefab, new Vector3(meatball.transform.position.x, 0.02f, meatball.transform.position.z), Quaternion.identity);
            Rigidbody rb = meatball.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = meatball.AddComponent<Rigidbody>();
            }

            rb.useGravity = true;

            elapsedTime += spawnInterval;

            if (elapsedTime >= speedUpInterval)
            {
                spawnInterval /= 2; 
                elapsedTime = 0f; 
            }
        }
    }
}
