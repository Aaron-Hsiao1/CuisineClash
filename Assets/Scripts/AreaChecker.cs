using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NewBehaviourScript : NetworkBehaviour
{
	[SerializeField] private KingOfTheGrillManager kingOfTheGrillManager;
	//[SerializeField] private ScoreManager scoreManager;

	private bool playerInArea = false;
	private float timer = 0f;
	private float delay = 1f;

	// Function called when another collider enters the trigger
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<NetworkBehaviour>().IsLocalPlayer) // Check if the entering collider has the "Player" tag
		{
			playerInArea = true;
			Debug.Log("player in area");

		}
	}

	// Function called when another collider exits the trigger
	private void OnTriggerExit(Collider other)
	{

		if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<NetworkBehaviour>().IsLocalPlayer) // Check if the exiting collider has the "Player" tag
		{
			playerInArea = false;
			timer = 0f;
			Debug.Log("player left area");
		}
	}
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (playerInArea)
		{
			timer += Time.deltaTime;
			if (timer >= delay)
			{
				if (kingOfTheGrillManager.IsGamePlaying())
				{
					kingOfTheGrillManager.AddScore(1);
				}

				timer = 0f; // Reset the timer
			}
		}
	}
}
