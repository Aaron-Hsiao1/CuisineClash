using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class Meatball : NetworkBehaviour
{
	private void OnTriggerEnter(Collider collision)
	{
		if (collision.gameObject.CompareTag("Ground") && IsServer)
		{
			DestroyMeatballServerRpc();
		}
	}

	[ServerRpc]
	private void DestroyMeatballServerRpc()
	{
		Destroy(gameObject);
	}
}
