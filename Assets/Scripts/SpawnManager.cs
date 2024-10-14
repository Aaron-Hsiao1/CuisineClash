using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;

public class SpawnManager : NetworkBehaviour
{
	public SpawnPoint[] spawnPoints;

	public override void OnNetworkSpawn()
	{
		spawnPoints = GetComponentsInChildren<SpawnPoint>();
		Debug.Log($"is spawn points null: {spawnPoints == null}"); ;
		Debug.Log($"SpawnPoints.Count: {spawnPoints.Count()}");
	}

	public Vector3 GetNextSpawnPoint()
	{
		foreach (var spawnPoint in spawnPoints)
		{
			if (spawnPoint.isAvailable)
			{
				spawnPoint.isAvailable = false;
				Vector3 _tempSpawnPoint = spawnPoint.transform.position;
				Debug.Log($"Spawn Point before addition: {_tempSpawnPoint}");
				_tempSpawnPoint += new Vector3(0, 2, 0);
				Debug.Log($"Spawn Point: {_tempSpawnPoint}");
				return _tempSpawnPoint;
			}
		}
		// Fallback if no spawn points are available
		Debug.Log("no spaw poitns ofund");
		return new Vector3(0, 0, 0);
	}
}
