using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KOTGDeathZone : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			Vector3 newPosition = new Vector3(-12.3f, 155.3f, -112.3f); // Replace with your desired teleport position
			other.transform.parent.position = newPosition;
		}
	}
}
