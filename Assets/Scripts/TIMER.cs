using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;


public class CountdownTimer : NetworkBehaviour
{
	/*public static CountdownTimer Instance { get; private set; }


	[SerializeField] private NetworkVariable<float> startTime = new NetworkVariable<float>(300f);
	[SerializeField] private TMP_Text timerText;
	[SerializeField] private bool gamePlaying = false;
	[SerializeField] private TMP_Text gameOverText;
	[SerializeField] private RainingMeatballManager rainingMeatballManager;

	private bool gameEnded;
	private NetworkVariable<float> currentTime = new NetworkVariable<float>(0f);

	public event EventHandler OnGameEnd;

	void Start()
	{
		OnGameEnd += CountdownTimer_OnGameEnd;

		//gameOverText = GameObject.Find("GameOverText2").GetComponent<TMP_Text>();
		if (gameOverText != null)
		{
			gameOverText.gameObject.SetActive(false);
		}
	}

	public override void OnNetworkSpawn()
	{
		currentTime.Value = startTime.Value;
		gamePlaying = true;
		gameEnded = false;
	}

	void Update()
	{
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

	private void CountdownTimer_OnGameEnd(object sender, EventArgs e)
	{
		EndGame();
	}

	[ClientRpc]
	private void UpdateTimerTextClientRpc()
	{
		UpdateTimerText();
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
		ShowGameOverText();
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

	private void ShowGameOverText()
	{
		if (gameOverText != null)
		{
			gameOverText.gameObject.SetActive(true);
		}

	}

	public bool GameEnded()
	{
		return gameEnded;
	}*/
}
