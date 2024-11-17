using System;
using UnityEngine;
using TMPro;
using Unity.Netcode;

//NOT USING (?)
public class HotPotatoTimer : NetworkBehaviour
{
	[SerializeField] private NetworkVariable<float> startTime = new NetworkVariable<float>(10f);
	private NetworkVariable<float> currentTime = new NetworkVariable<float>(0f);
	[SerializeField] private TMP_Text timerText;

	private float timeBeforeExplosion;

	public event EventHandler PotatoTimerEnd;

	#region Variables

	private TMP_Text _timerText;
	enum TimerType { Countdown, Stopwatch }
	[SerializeField] private TimerType timerType;

	private float timeToDisplay = 10.0f;
	private float initialTime; // Store the initial time value to reset later
	private bool _isRunning;
	public float repeatDelay = 5.0f; // Time to wait before repeating the timer

	#endregion

	private void Update()
	{
		if (!IsHost)
		{
			return;
		}

		currentTime.Value -= Time.deltaTime;

		if (currentTime.Value <= 0)
		{
			currentTime.Value = 0;
			PotatoTimerEnd?.Invoke(this, EventArgs.Empty);
		}

		UpdateTimerTextClientRpc();
	}

	public override void OnNetworkSpawn()
	{
		PotatoTimerEnd += HotPotatoTimer_PotatoTimerEnd;

		startTime.Value = timeBeforeExplosion;
		currentTime.Value = startTime.Value;
	}

	private void HotPotatoTimer_PotatoTimerEnd(object sender, EventArgs e)
	{
		Invoke(nameof(RestartTimer), repeatDelay);
		KillPlayers();
	}

	private void KillPlayers()
	{
		Debug.Log("Client ID: " + NetworkManager.Singleton.LocalClientId);

		HotPotatoExplosion[] players = FindObjectsOfType<HotPotatoExplosion>(); // Assuming you have multiple players

		foreach (var player in players)
		{
			Debug.Log("Foreach Loop");

			if (player.HasHotPotato()) // Check if this player has the active hot potato
			{
				Debug.Log("EXPLOSION");
				player.Eliminate(); // Call the player's elimination method	
				break;
			}
		}
	}

	private void Awake()
	{
		timeBeforeExplosion = 10f;
		Debug.Log("Timer Awake!");
		_timerText = GetComponent<TMP_Text>();
		initialTime = timeToDisplay; // Store the initial time at start
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

		timeToDisplay = initialTime; // Reset the timer
		_timerText.enabled = true; // Show the text again
		_isRunning = true; // Restart the timer
	}
}