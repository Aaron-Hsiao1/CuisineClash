using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;
using System.Linq;

public class KingOfTheGrillManager : NetworkBehaviour
{
	public static KingOfTheGrillManager Instance { get; private set; }

	private float totalTime = 100f; //Total Game Time in seconds

	[SerializeField] private GameObject pillarPrefab; // Prefab of the pillar to spawn
	[SerializeField] private GameObject fireIndicatorPrefab;
	[SerializeField] private float moveSpeed = 1000f;    // Speed at which the pillar moves upwards
	[SerializeField] private float stopHeight = 10f; // Height at which the pillar will be deleted
	[SerializeField] private float indToSpawnDelay = 3f;

	[SerializeField] private bool gamePlaying;
	private bool gameEnded;
	private NetworkVariable<float> currentTime = new NetworkVariable<float>(0f);
	[SerializeField] private TMP_Text timerText;
	[SerializeField] private NetworkVariable<float> startTime = new NetworkVariable<float>(10f);

	[SerializeField] private TMP_Text gameOverText;
	[SerializeField] private GameObject leaderboard;
	[SerializeField] private TMP_Text leaderboardText;
	[SerializeField] private TMP_Text scoreText;

	//[SerializeField] private Camera secondaryCamera;
	[SerializeField] private CuisineClashManager cuisineClashManager;
	private CuisineClashMultiplayer cuisineClashMultiplayer;

	public event EventHandler OnGameEnd;

	private Vector3 indSpawn = new Vector3(-14, -52, -1);
	private Vector3 boxSpawn = new Vector3(-14, -132, -1);
	private float delay = 2f;

	private float spawnDelay = 2f;
	private float elapsedTime;

	private Dictionary<ulong, int> playerScores = new Dictionary<ulong, int>();

	[SerializeField] private ScoreManager scoreManager;

	private void Start()
	{
		elapsedTime = 0f;

		OnGameEnd += KingOfTheGrillManager_OnGameEnd;
	}


	public override void OnNetworkSpawn()
	{
		startTime.Value = totalTime;
		currentTime.Value = startTime.Value;
		gamePlaying = true;
		gameEnded = false;
		leaderboardText.text = "";
		leaderboard.SetActive(false);
		gameOverText.gameObject.SetActive(false);

		cuisineClashMultiplayer = GameObject.Find("CuisineClashMultiplayer").GetComponent<CuisineClashMultiplayer>();
		//Debug.Log("cuisine clash multipalyer = null; " + cuisineClashMultiplayer == null);

		foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) //populates player score dictionary
		{
			playerScores.Add(clientId, 0);
		}
		if (!IsHost)
		{
			return;
		}


		SpawnPillarClientRpc();
	}

	private void Update()
	{
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
	}

	private void KingOfTheGrillManager_OnGameEnd(object sender, EventArgs e)
	{
		EndGameClientRpc();
	}

	private void UpdateTimerText()
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

	[ClientRpc]
	private void EndGameClientRpc()
	{
		CalculatePoints();

		EndGame();
		Debug.Log("timer.gameEnded()");
	}

	public void EndGame()
	{
		gamePlaying = false;
		gameEnded = true;
		StartCoroutine(ShowEndGameUIs());
		//Destroy(gameObject);
	}

	IEnumerator ShowEndGameUIs()
	{
		Cursor.lockState = CursorLockMode.None;
		timerText.gameObject.SetActive(false);
		//secondaryCamera.gameObject.SetActive(true);
		gameOverText.gameObject.SetActive(true);
		yield return new WaitForSeconds(1f);
		gameOverText.gameObject.SetActive(false);
		UpdateLeaderboardClientRpc();
		leaderboard.SetActive(true);
		yield return new WaitForSeconds(3f);

		if (GamemodeManager.Instance.GetGamemodeList().Count > 0)
		{
			Loader.LoadNetwork(Loader.Scene.PregameLobby.ToString());
		}
		if (GamemodeManager.Instance.GetGamemodeList().Count == 0)
		{
			Loader.LoadNetwork(Loader.Scene.GameEnded.ToString());
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

	IEnumerator InstantiatePillar()
	{
		Vector3 pillarSpawnPoint = GetPillarSpawnPoint();

		GameObject indicatorPillar = Instantiate(fireIndicatorPrefab, pillarSpawnPoint, Quaternion.identity);
		var indicatorPillarNetworkObject = indicatorPillar.GetComponent<NetworkObject>();
		indicatorPillarNetworkObject.Spawn(true);

		yield return new WaitForSeconds(1);

		GameObject pillar = Instantiate(pillarPrefab, pillarSpawnPoint, Quaternion.identity);
		var pillarNetworkObject = pillar.GetComponent<NetworkObject>();
		pillarNetworkObject.Spawn(true);

		indicatorPillarNetworkObject.Despawn(true);
		Destroy(indicatorPillar);

		pillar.transform.parent = transform;

		// Start moving the pillar upwards
		StartCoroutine(MovePillar(pillar.transform, indicatorPillar.transform));


	}

	IEnumerator MovePillar(Transform pillarTransform, Transform indicatorTransform)
	{
		//yield return new WaitForSeconds(indToSpawnDelay);
		//Destroy(indicatorTransform.gameObject);
		//indicatorTransform.gameObject.GetComponent<NetworkObject>().Despawn(true); 

		while (pillarTransform.position.y < stopHeight)
		{
			// Move the pillar upwards
			pillarTransform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

			// Wait for the next frame
			yield return null;
		}

		yield return new WaitForSeconds(delay);

		Destroy(pillarTransform.gameObject);
		pillarTransform.gameObject.GetComponent<NetworkObject>().Despawn(true);
	}

	IEnumerator SpawnPillarLoop()
	{
		while (true)
		{
			yield return new WaitForSeconds(spawnDelay);

			StartCoroutine(InstantiatePillar());

			elapsedTime += spawnDelay; //adds time to the elapsed time

			if (elapsedTime >= spawnDelay) // if the elapsed time is greater than the time needed to speed up, spawn cd is halved, and elapsed time resets
			{
				elapsedTime = 0f;
			}
		}
	}

	private Vector3 GetPillarSpawnPoint()
	{
		float _radius = 90;
		float _grillY = 115;


		//center: -12.7, Y, -114.8
		Vector3 _centerPoint = new Vector3(-12.7f, 0, -114.8f);
		//57.2, -81.9

		//77.25 radius

		//Vector2 randomPoint = centerPoint + Random.insideUnitCircle * radius * 0.5f;

		Vector3 randomSpawnPoint = _centerPoint += _radius * 0.5f * UnityEngine.Random.insideUnitSphere;
		randomSpawnPoint.y = _grillY;
		return randomSpawnPoint;
	}

	[ClientRpc]
	private void SpawnPillarClientRpc()
	{
		StartCoroutine(SpawnPillarLoop());
		Debug.Log("spawn pillar client rpc");
	}

	[ServerRpc]
	private void SpawnPillarServerRpc()
	{
		SpawnPillarClientRpc();
		Debug.Log("spawn pillar server rpc");
	}

	public bool IsGamePlaying()
	{
		return gamePlaying;
	}

	private void CalculatePoints()
	{
		var playerScoresSorted = playerScores.OrderByDescending(player => player.Value);
		List<int> times = playerScores.Select(p => p.Value).ToList();

		for (int i = 0; i < playerScoresSorted.Count(); i++)
		{
			if (i == 0) //first place
			{
				if (times.Count(t => t == times[i]) == 1) //counts how many times t are equal to times[i] (basically checking for ties)
				{
					//+3 points
					foreach (var player in playerScoresSorted)
					{
						if (player.Value == times[i])
						{
							cuisineClashMultiplayer.AddPoints(player.Key, 3);
						}
					}

				}
				else if (times.Count(t => t == times[i]) == 2) //2 weay tie
				{
					// +2 points for top 2
					foreach (var player in playerScoresSorted)
					{
						if (player.Value == times[i])
						{
							cuisineClashMultiplayer.AddPoints(player.Key, 2);
						}
					}
					i++;
				}
				else if (times.Count(t => t == times[t]) == 3) //3 way tie
				{
					// +1 piont for top 3
					foreach (var player in playerScoresSorted)
					{
						if (player.Value == times[i])
						{
							cuisineClashMultiplayer.AddPoints(player.Key, 1);
						}
					}
					i += 2;
				}
			}
			else if (i == 1 && times.Count(t => t == times[i]) == 1)//2nd place with no ties
			{
				// +2 points for 2nd place
				foreach (var player in playerScoresSorted)
				{
					if (player.Value == times[i])
					{
						cuisineClashMultiplayer.AddPoints(player.Key, 2);
					}
				}
			}
			else if (i == 2 && times.Count(t => t == times[i]) == 1)//3rd place with no ties
			{
				// + 1 point for 3rd
				foreach (var player in playerScoresSorted)
				{
					if (player.Value == times[i])
					{
						cuisineClashMultiplayer.AddPoints(player.Key, 1);
					}
				}
			}
		}
	}

	public void AddScore(int score)
	{
		AddScoreServerRpc(score);
	}

	[ServerRpc(RequireOwnership = false)]
	private void AddScoreServerRpc(int score, ServerRpcParams serverRpcParams = default)
	{
		var sender = serverRpcParams.Receive.SenderClientId;

		playerScores[sender] += score;
		UpdateScoreText(sender, playerScores[sender]);

	}

	[ClientRpc]
	private void UpdateScoreTextClientRpc(ulong clientId, int score)
	{
		if (clientId != NetworkManager.Singleton.LocalClientId)
		{
			return;
		}
		scoreText.text = "Score: " + score;
	}

	private void UpdateScoreText(ulong clientId, int score)
	{
		UpdateScoreTextClientRpc(clientId, score);
	}
}
