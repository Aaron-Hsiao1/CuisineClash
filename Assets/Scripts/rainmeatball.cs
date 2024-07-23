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
		Vector3 spawnPosition = new Vector3(
				Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2),
				spawnHeight,
				Random.Range(-spawnAreaLength / 2, spawnAreaLength / 2)
			); //finds spawn location

		GameObject meatball = Instantiate(meatballPrefab, spawnPosition, Quaternion.identity); //Instantiates meatball
		var meatballNetworkObject = meatball.GetComponent<NetworkObject>();
		meatballNetworkObject.Spawn(true); //spawns meatball on server

		GameObject indicator = Instantiate(indicatorPrefab, new Vector3(meatball.transform.position.x, 0.02f, meatball.transform.position.z), Quaternion.identity); //Instantiates indicator
		var indicatorNetworkObject = indicator.GetComponent<NetworkObject>();
		indicatorNetworkObject.Spawn(true);
	}
}
