using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;

public class HotPotatoManager : NetworkBehaviour
{
	private NetworkVariable<ulong> currentPlayerWithPotato = new NetworkVariable<ulong>(0);

	private List<ulong> alivePlayerIds;

	//Timer things
	[SerializeField] private NetworkVariable<float> startTime = new NetworkVariable<float>(10f);
	private NetworkVariable<float> currentTime = new NetworkVariable<float>(0f);
	private NetworkVariable<bool> timerRunning = new NetworkVariable<bool>(true);
	[SerializeField] private TMP_Text timerText;
	public float repeatDelay = 5.0f;

	private float timeBeforeExplosion;

	public event EventHandler PotatoTimerEnd;

	[SerializeField] private Player[] players;

	public override void OnNetworkSpawn()
	{
		CuisineClashManager.Instance.AllPlayerObjectsSpawned += HotPotatoManager_AllPlayerObjectsSpawned;

		PotatoTimerEnd += HotPotatoManager_PotatoTimerEnd;

		startTime.Value = timeBeforeExplosion;
		currentTime.Value = startTime.Value;
	}

	private void Update()
	{
		if (!IsHost)
		{
			return;
		}

		currentTime.Value -= Time.deltaTime;

		if (currentTime.Value <= 0 && timerRunning.Value)
		{
			currentTime.Value = 0;
			timerRunning.Value = false;
			PotatoTimerEnd?.Invoke(this, EventArgs.Empty);
		}

		UpdateTimerTextClientRpc();
	}

	private void Awake()
	{
		timeBeforeExplosion = 10f;
		Debug.Log("Timer Awake!");
	}

	private void HotPotatoManager_PotatoTimerEnd(object sender, EventArgs e)
	{
		KillPlayers();
		HideTimerTextClientRpc();
		Invoke(nameof(RestartTimer), repeatDelay);
	}

	[ClientRpc]
	private void HideTimerTextClientRpc()
	{
		timerText.gameObject.SetActive(false);
	}

	[ClientRpc]
	private void ShowTimerTextClientRpc()
	{
		timerText.gameObject.SetActive(true);
	}

	private void KillPlayers()
	{
		Debug.Log("Client ID: " + NetworkManager.Singleton.LocalClientId);

		HotPotatoExplosion[] players = FindObjectsOfType<HotPotatoExplosion>(); // Assuming you have multiple players

		foreach (var player in players)
		{
			Debug.Log("Foreach Loop");
			Debug.Log("player.haspotato: " + player.HasHotPotato());
			if (player.HasHotPotato()) // Check if this player has the active hot potato
			{
				Debug.Log("EXPLOSION");
				player.Eliminate(); // Call the player's elimination method	
				break;
			}
		}

		/*
			HotPotatoExplosion[] players = FindObjectsOfType<HotPotatoExplosion>(); // Assuming you have multiple players

			foreach (var player in players)
			{
				Debug.Log("Foreach Loop");


			}*/
	}

	void UpdateTimerText()
	{
		int minutes = Mathf.FloorToInt(currentTime.Value / 60);
		int seconds = Mathf.FloorToInt(currentTime.Value % 60);
		timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
	}

	[ClientRpc]
	private void UpdateTimerTextClientRpc()
	{
		UpdateTimerText();
	}

	private void RestartTimer()
	{
		currentTime.Value = timeBeforeExplosion;
		timerRunning.Value = true;
		ShowTimerTextClientRpc();

		//timeToDisplay = initialTime; // Reset the timer
		//_timerText.enabled = true; // Show the text again
		//_isRunning = true; // Restart the timer
	}

	private void HotPotatoManager_AllPlayerObjectsSpawned(object sender, EventArgs e)
	{
		Debug.Log("All players loaded in!");
		// This logic ensures that playerIDs are only populated once when the network is spawned
		if (IsServer)
		{
			alivePlayerIds = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);

			// Assign the hot potato to a random player at the start if enough players
			if (alivePlayerIds.Count > 1)
			{
				AssignRandomPlayerWithPotato();
			}
		}
	}

	private void AssignRandomPlayerWithPotato() //Server selects a random player to recieve hot potato
	{
		int randomPlayer = UnityEngine.Random.Range(0, alivePlayerIds.Count);
		currentPlayerWithPotato.Value = alivePlayerIds[randomPlayer];

		Debug.Log("players length: " + alivePlayerIds.Count);
		Debug.Log("New player with hot potato: " + currentPlayerWithPotato);

		// Inform all clients who now has the potato
		SetHotPotato(currentPlayerWithPotato.Value);
	}

	public void SetHotPotato(ulong potatoHolderId) //server sets hot potato to active based on who has it
	{
		SetHotPotatoClientRpc(potatoHolderId);
	}

	[ClientRpc]
	public void SetHotPotatoClientRpc(ulong potatoHolderId)  //client sets hot potato to active based on potatoholderid
	{
		//Debug.Log("if statemetn ran");
		//Client cannot access connectedclients
		players = FindObjectsOfType<Player>(); // Assuming you have multiple players
		foreach (var player in players)
		{
			Debug.Log("GetClientID: " + player.GetClientId()); //server detecting client id on client 1 is 0 when it should be 1
			Debug.Log("Potato Holder Id: " + potatoHolderId);
			GameObject potatoObject = player.gameObject.transform.Find("PlayerObj/CHACTER1animationattempt/temppotato").gameObject;
			if (player.GetClientId() == potatoHolderId)
			{
				Debug.Log("Setting hot potato active on player: " + player.GetClientId());

				potatoObject.SetActive(true);
			}
			else
			{
				potatoObject.SetActive(false);
			}
		}
	}

	public void OnPotatoExploded()
	{
		StartCoroutine(ReassignPotatoAfterCooldown());
	}

	private IEnumerator ReassignPotatoAfterCooldown()
	{
		Debug.Log("Reassigning potato");
		yield return new WaitForSeconds(repeatDelay); // Wait for the cooldown time
		AssignRandomPlayerWithPotato(); // Reassign to a random player
	}

	public ulong GetPlayerWithPotato()
	{
		return currentPlayerWithPotato.Value;
	}

	public override void OnNetworkDespawn()
	{
		CuisineClashManager.Instance.AllPlayerObjectsSpawned -= HotPotatoManager_AllPlayerObjectsSpawned;
	}
}
