using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class KOTGDeathZone : NetworkBehaviour
{
	[SerializeField] private SpawnManager spawnManager;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			Vector3 newPosition = spawnManager.GetRandomSpawnPoint() + new Vector3(0, 2, 0);
			other.transform.parent.position = newPosition;
		}
	}
}
