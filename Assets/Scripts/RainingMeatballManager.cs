using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using TMPro;
using Unity.Collections;

[GenerateSerializationForType(typeof(string))]
public class RainingMeatballManager : NetworkBehaviour, INetworkSerializeByMemcpy
{
	//Timer stuff
	[SerializeField] private NetworkVariable<float> startTime = new NetworkVariable<float>(10f);
	[SerializeField] private TMP_Text timerText;
	[SerializeField] private bool gamePlaying = false;
	[SerializeField] private TMP_Text gameOverText;
	[SerializeField] private RainingMeatballManager rainingMeatballManager;

	[SerializeField] private GameObject leaderboard;
	[SerializeField] private TMP_Text leaderboardText;
	//private NetworkVariable<FixedString128Bytes> leaderboardString = new NetworkVariable<FixedString128Bytes>("");

	[SerializeField] private Camera secondaryCamera;

	private bool gameEnded;
	private NetworkVariable<float> currentTime = new NetworkVariable<float>(0f);

	public event EventHandler OnGameEnd;

	private float totalTime = 300f; //Total Game Time in seconds

	// Meatball manager stuff
	private List<ulong> alivePlayers;
	private Dictionary<int, ulong> playerPlacements;
	//private CuisineClashManager cuisineClashManager;
	private CuisineClashMultiplayer cuisineClashMultiplayer;

	private void Start()
	{
		OnGameEnd += RainingMeatballManager_OnGameEnd;

		if (gameOverText != null)
		{
			gameOverText.gameObject.SetActive(false);
		}
	}

	public override void OnNetworkSpawn()
	{
		leaderboardText.text = "";

		startTime.Value = totalTime;
		currentTime.Value = startTime.Value;
		gamePlaying = true;
		gameEnded = false;

		alivePlayers = new List<ulong>();
		playerPlacements = new Dictionary<int, ulong>();

		cuisineClashMultiplayer = GameObject.Find("CuisineClashMultiplayer").GetComponent<CuisineClashMultiplayer>();

		if (IsServer)
		{
			foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
			{
				alivePlayers.Add(clientId);
			}
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.M))
		{
			Debug.Log($"player points.count: {cuisineClashMultiplayer.GetPlayerPoints().Count}");
		}
		if (!IsHost)
		{
			return;
		}

		if (gamePlaying && !gameEnded)
		{
			currentTime.Value -= Time.deltaTime;

			if (currentTime.Value <= 0)
			{
				currentTime.Value = 0;
				OnGameEnd?.Invoke(this, EventArgs.Empty);
			}

			UpdateTimerTextClientRpc();
		}

		if (alivePlayers.Count == 0 && !gameEnded)
		{
			OnGameEnd?.Invoke(this, EventArgs.Empty);
			Debug.Log("Alive players = 0");
		}

	}

	private void RainingMeatballManager_OnGameEnd(object sender, EventArgs e)
	{
		EndGameClientRpc();
	}

	private void RainingMeatballManager_OnLeaderboardStringChanged(FixedString128Bytes oldValue, FixedString128Bytes newValue)
	{
		// Convert the FixedString128Bytes to a string
		string tempString = newValue.ToString();

		// Update the TMP_Text component with the new text
		leaderboardText.text = tempString;
		Debug.Log($"Leaderboard updated to: {tempString}");
	}

	[ClientRpc]
	private void UpdateTimerTextClientRpc()
	{
		UpdateTimerText();
	}

	[ClientRpc]
	private void EndGameClientRpc()
	{
		CalculatePoints();
		EndGame();
		Debug.Log("timer.gameEnded()");
	}

	void UpdateTimerText()
	{
		int minutes = Mathf.FloorToInt(currentTime.Value / 60);
		int seconds = Mathf.FloorToInt(currentTime.Value % 60);
		timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
	}

	public void EndGame()
	{
		gamePlaying = false;
		gameEnded = true;
		StartCoroutine(ShowEndGameUIs());

		//Destroy(gameObject);
	}

	public void StartTimer(float newTime)
	{
		startTime.Value = newTime;
		currentTime = startTime;
		gamePlaying = true;
	}

	public void StopTimer()
	{
		gamePlaying = false;
	}

	public bool GameEnded()
	{
		return gameEnded;
	}

	public void EliminatePlayer(ulong clientId) // Only when game is running
	{
		playerPlacements[alivePlayers.Count] = clientId;
		alivePlayers.Remove(clientId);
		Debug.Log("Player eliminated: " + clientId);
	}

	public int AlivePlayerCount()
	{
		return alivePlayers.Count;
	}

	public Dictionary<int, ulong> PlayerPlacements()
	{
		return playerPlacements;
	}

	IEnumerator ShowEndGameUIs()
	{
		Cursor.lockState = CursorLockMode.None;
		timerText.gameObject.SetActive(false);
		secondaryCamera.gameObject.SetActive(true);
		gameOverText.gameObject.SetActive(true);
		yield return new WaitForSeconds(1f);
		gameOverText.gameObject.SetActive(false);
		UpdateLeaderboardClientRpc();
		leaderboard.SetActive(true);

		yield return new WaitForSeconds(3f);

		if (GamemodeManager.Instance.GetGamemodeList().Count > 0)
		{
			Loader.LoadNetwork(Loader.Scene.PregameLobby);
		}
		if (GamemodeManager.Instance.GetGamemodeList().Count == 0)
		{
			Loader.LoadNetwork(Loader.Scene.GameEnded);
		}
	}

	private void UpdateLeaderboard()
	{
		Debug.Log("updating leaderboard...");
		foreach (KeyValuePair<ulong, int> player in cuisineClashMultiplayer.GetPlayerPoints())
		{
			var playerName = CuisineClashMultiplayer.Instance.GetPlayerDataFromClientId(player.Key).playerName;
			leaderboardText.text += $"{playerName}: {player.Value}\n";
		}
		Debug.Log($"leaderboradString: {leaderboardText.text}");
		//leaderboardText.text = leaderboardString.Value.ToString();
	}

	[ClientRpc]
	private void UpdateLeaderboardClientRpc()
	{
		UpdateLeaderboard();
	}

	private void CalculatePoints()
	{
		if (alivePlayers.Count <= 1)
		{
			for (int i = 1; i <= 3; i++)
			{
				if (playerPlacements.TryGetValue(i, out ulong clientId))
				{
					cuisineClashMultiplayer.AddPoints(clientId, 4 - i);
				}
			}
		}
		Debug.Log("1 winner");
		Debug.Log("playerplacements[1] : " + playerPlacements.ContainsKey(1));
		Debug.Log("playerplacements[2] : " + playerPlacements.ContainsKey(2));
		Debug.Log("playerplacements[3] : " + playerPlacements.ContainsKey(3));
		if (alivePlayers.Count == 2) // 2 people alive
		{
			Debug.Log("2 winner");
			foreach (ulong player in alivePlayers)
			{
				cuisineClashMultiplayer.AddPoints(player, 2);
			}
		}
		else if (alivePlayers.Count >= 3) // 3 or more people alive
		{
			Debug.Log("3 winner");
			foreach (ulong player in alivePlayers)
			{
				cuisineClashMultiplayer.AddPoints(player, 1);
			}
		}
	}



}
