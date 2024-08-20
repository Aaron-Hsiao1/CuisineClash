using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class Indicator : NetworkBehaviour
{
	private void OnTriggerEnter(Collider trigger)
	{
		if (trigger.gameObject.CompareTag("Meatball") && IsServer)
		{
			DestroyIndicatorServerRpc();
		}
	}

	[ServerRpc]
	private void DestroyIndicatorServerRpc()
	{
		Destroy(gameObject);
	}
}
