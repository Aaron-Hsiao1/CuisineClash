using System.Collections;
using UnityEngine;

public class MeatballSpawner : MonoBehaviour
{
    public GameObject meatballPrefab; 
    public float spawnAreaWidth = 10f; 
    public float spawnAreaLength = 10f; 
    public float spawnHeight = 20f; 
    public float spawnInterval = 1f; 

    void Start()
    {
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

            Rigidbody rb = meatball.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = meatball.AddComponent<Rigidbody>();
            }

            rb.useGravity = true;
        }
    }
}
