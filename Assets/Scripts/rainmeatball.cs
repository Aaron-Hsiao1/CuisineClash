using System.Collections;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class MeatballSpawner : NetworkBehaviour
{
	public GameObject meatballPrefab;
	public GameObject indicatorPrefab;
	[SerializeField] private float spawnAreaWidth = 10f;
	[SerializeField] private float spawnAreaLength = 10f;
	[SerializeField] private float spawnHeight = 20f;
	[SerializeField] private float initialSpawnInterval = 1f;
	[SerializeField] private float speedUpInterval = 60f;

	private float spawnInterval; //current spawn interval
	private float elapsedTime;

	void Start()
	{
		spawnInterval = initialSpawnInterval;
		elapsedTime = 0f;

	}
	public override void OnNetworkSpawn()
	{
		if (!IsHost)
		{
			return;
		}
		SpawnMeatballServerRpc();
	}

	[ServerRpc]
	private void SpawnMeatballServerRpc()
	{
		SpawnMeatballClientRpc();
	}

	[ClientRpc]
	private void SpawnMeatballClientRpc()
	{
		StartCoroutine(SpawnMeatballLoop());
	}

	IEnumerator SpawnMeatballLoop()
	{
		while (true)
		{
			yield return new WaitForSeconds(spawnInterval);

			SpawnMeatball();

			elapsedTime += spawnInterval; //adds time to the elapsed time

			if (elapsedTime >= speedUpInterval) // if the elapsed time is greater than the time needed to speed up, spawn cd is halved, and elapsed time resets
			{
				spawnInterval /= 2;
				elapsedTime = 0f;
			}
		}
	}

    private void SpawnMeatball()
    {
        // Randomly generate spawn position within defined area
        Vector3 spawnPosition = new Vector3(
            Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2),
            spawnHeight,
            Random.Range(-spawnAreaLength / 2, spawnAreaLength / 2)
        );

        // Instantiate meatball
        GameObject meatball = Instantiate(meatballPrefab, spawnPosition, Quaternion.identity);
        var meatballNetworkObject = meatball.GetComponent<NetworkObject>();
        meatballNetworkObject.Spawn(true); // Spawns meatball on server

        // Raycast to find terrain height at the x and z coordinates of the meatball
        RaycastHit hit;
        Vector3 indicatorPosition = new Vector3(meatball.transform.position.x, 0, meatball.transform.position.z);
        if (Physics.Raycast(meatball.transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            // Adjust indicator's y position based on terrain height, add a vertical offset
            indicatorPosition.y = hit.point.y + 0.2f; // Offset added to ensure indicator is above ground

            // Optional: Adjust rotation to align with surface normal (useful on slants)
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            // Instantiate the indicator at the correct position and rotation
            GameObject indicator = Instantiate(indicatorPrefab, indicatorPosition, rotation);
            var indicatorNetworkObject = indicator.GetComponent<NetworkObject>();
            indicatorNetworkObject.Spawn(true);
        }
        else
        {
            // Default to a low height if no terrain is hit, but still add a small offset to make it visible
            indicatorPosition.y = 0.2f;
            GameObject indicator = Instantiate(indicatorPrefab, indicatorPosition, Quaternion.identity);
            var indicatorNetworkObject = indicator.GetComponent<NetworkObject>();
            indicatorNetworkObject.Spawn(true);
        }
    }


}
