using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;
using UnityEngine.SocialPlatforms.Impl;

public class HotPotatoManager : NetworkBehaviour
{
	public NetworkVariable<ulong> currentPlayerWithPotato = new NetworkVariable<ulong>();

	private List<ulong> alivePlayerIds;
	private List<ulong> topThreePlayers;

	//Timer things
	[SerializeField] private NetworkVariable<float> startTime = new NetworkVariable<float>(10f);
	private NetworkVariable<float> currentTime = new NetworkVariable<float>(0f);
	private NetworkVariable<bool> timerRunning = new NetworkVariable<bool>(true);
	[SerializeField] private TMP_Text timerText;
	public float repeatDelay = 5.0f;

	private float timeBeforeExplosion;

	public event EventHandler PotatoTimerEnd;

	[SerializeField] private Player[] players;

	//End Game UIs
    [SerializeField] private Camera secondaryCamera;
    [SerializeField] private TMP_Text gameOverText;
    private CuisineClashMultiplayer cuisineClashMultiplayer;
    [SerializeField] private GameObject leaderboard;
    [SerializeField] private TMP_Text leaderboardText;

    public event EventHandler OnGameEnd;

    public override void OnNetworkSpawn()
	{
		CuisineClashManager.Instance.AllPlayerObjectsSpawned += HotPotatoManager_AllPlayerObjectsSpawned;
        cuisineClashMultiplayer = GameObject.Find("CuisineClashMultiplayer").GetComponent<CuisineClashMultiplayer>();

        PotatoTimerEnd += HotPotatoManager_PotatoTimerEnd;

		startTime.Value = timeBeforeExplosion;
		currentTime.Value = startTime.Value;

        leaderboardText.text = "";
    }
    public override void OnNetworkDespawn()
    {
        CuisineClashManager.Instance.AllPlayerObjectsSpawned -= HotPotatoManager_AllPlayerObjectsSpawned;
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

    private void Start()
    {
        OnGameEnd += HotPotatoManager_OnGameEnd;
    }

    private void Awake()
	{
		timeBeforeExplosion = 100000f;
		topThreePlayers = new List<ulong>();
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

				if (alivePlayerIds.Count <= 3)
				{
					topThreePlayers.Insert(0, currentPlayerWithPotato.Value);
				}

                alivePlayerIds.Remove(currentPlayerWithPotato.Value); //Remove from alive players

                if (alivePlayerIds.Count == 0)
				{
					OnGameEnd?.Invoke(this, EventArgs.Empty);
				}

                StartCoroutine(ReassignPotatoAfterCooldown());
                break;
			}
		}
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

    private void HotPotatoManager_OnGameEnd(object sender, EventArgs e)
    {
		EndGameClientRpc();
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
        StartCoroutine(ShowEndGameUIs());
    }

	private void CalculatePoints()
	{
		for (int i = 0; i < topThreePlayers.Count; i++)
		{
			cuisineClashMultiplayer.AddPoints(topThreePlayers[i], 3 - i);
		}
	}


    [ClientRpc]
    private void UpdateLeaderboardClientRpc()
    {
        UpdateLeaderboard();
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
		if (alivePlayerIds.Count == 0)
		{
			return;
		}
		
		int randomPlayer = UnityEngine.Random.Range(0, alivePlayerIds.Count);
		currentPlayerWithPotato.Value = alivePlayerIds[randomPlayer];

		Debug.Log("players length: " + alivePlayerIds.Count);
		Debug.Log("New player with hot potato: " + currentPlayerWithPotato.Value);

        // Inform all clients who now has the potato
        SetHotPotatoActive(currentPlayerWithPotato.Value);
	}

	public void SetHotPotatoActive(ulong potatoHolderId) //server sets hot potato to active based on who has it
	{
        SetHotPotatoActiveClientRpc(potatoHolderId);
	}

	[ClientRpc]
	public void SetHotPotatoActiveClientRpc(ulong potatoHolderId)  //client sets hot potato to active based on potatoholderid
	{
		players = FindObjectsOfType<Player>(); // Assuming you have multiple players
		foreach (var player in players)
		{
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

    public void SetHotPotatoInactive(ulong potatoHolderId)
	{
		SetHotPotatoInactiveClientRpc(potatoHolderId);

    }

    [ClientRpc]
	private void SetHotPotatoInactiveClientRpc(ulong potatoHolderId)
	{
        players = FindObjectsOfType<Player>(); // Assuming you have multiple players
        foreach (var player in players)
        {
            GameObject potatoObject = player.gameObject.transform.Find("PlayerObj/CHACTER1animationattempt/temppotato").gameObject;
            if (player.GetClientId() == potatoHolderId)
            {
                Debug.Log("Setting hot potato inactive on player: " + player.GetClientId());

				potatoObject.SetActive(false);
            }
        }
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

	public void TransferHotPotato(ulong newPotatoHolderId){
		TransferHotPotatoServerRpc(newPotatoHolderId);
	}

	[ServerRpc(RequireOwnership = false)]
	public void TransferHotPotatoServerRpc(ulong newPotatoHolderId)
	{
        Debug.Log("transferring potato to: " + newPotatoHolderId);
		Debug.Log("Is server?" + IsServer);	
        SetHotPotatoInactive(currentPlayerWithPotato.Value);
        SetHotPotatoActive(newPotatoHolderId);
        currentPlayerWithPotato.Value = newPotatoHolderId;
        Debug.Log("Current player with potato after transfer: " + currentPlayerWithPotato.Value);
        
    }
}
