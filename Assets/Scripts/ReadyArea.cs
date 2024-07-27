using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ReadyArea : NetworkBehaviour
{
	public CuisineClashManager cuisineClashManager;

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log(other.gameObject.GetComponent<NetworkBehaviour>() == null);
		if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<NetworkBehaviour>().IsLocalPlayer)
		{
			cuisineClashManager.SetIsLocalPlayerReady();
			cuisineClashManager.SetPlayerReady();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<NetworkBehaviour>().IsLocalPlayer)
		{
			cuisineClashManager.SetIsLocalPlayerUnready();
			cuisineClashManager.SetPlayerUnready();
		}
	}
}
