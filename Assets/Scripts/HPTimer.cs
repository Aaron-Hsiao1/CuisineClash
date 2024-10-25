using System;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    #region Variables

    private TMP_Text _timerText;
    enum TimerType { Countdown, Stopwatch }
    [SerializeField] private TimerType timerType;

    [SerializeField] private float timeToDisplay = 60.0f;
    private float initialTime; // Store the initial time value to reset later
    private bool _isRunning;
    private float repeatDelay = 5.0f; // Time to wait before repeating the timer

    #endregion

    private void Awake()
    {
        _timerText = GetComponent<TMP_Text>();
        initialTime = timeToDisplay; // Store the initial time at start
    }

    private void OnEnable()
    {
        EventManager.TimerStart += EventManagerOnTimerStart;
        EventManager.TimerStop += EventManagerOnTimerStop;
        EventManager.TimerUpdate += EventManagerOnTimerUpdate;
    }

    private void OnDisable()
    {
        EventManager.TimerStart -= EventManagerOnTimerStart;
        EventManager.TimerStop -= EventManagerOnTimerStop;
        EventManager.TimerUpdate -= EventManagerOnTimerUpdate;
    }

    private void EventManagerOnTimerStart() => _isRunning = true;
    private void EventManagerOnTimerStop() => _isRunning = false;
    private void EventManagerOnTimerUpdate(float value) => timeToDisplay += value;

    private void Update()
    {
        if (!_isRunning) return;

        if (timerType == TimerType.Countdown && timeToDisplay < 0.0f)
        {
            _isRunning = false; // Stop the timer
            _timerText.enabled = false; // Hide the text

            // Find all players and check which one has the active hot potato
            HPDeathTimer[] players = FindObjectsOfType<HPDeathTimer>(); // Assuming you have multiple players

            foreach (var player in players)
            {
                if (player.HasHotPotato()) // Check if this player has the active hot potato
                {
                    player.Eliminate(); // Call the player's elimination method
                    break;
                }
            }

            Invoke(nameof(RestartTimer), repeatDelay); // Restart after a delay
            return;
        }

        timeToDisplay += timerType == TimerType.Countdown ? -Time.deltaTime : Time.deltaTime;

        TimeSpan timeSpan = TimeSpan.FromSeconds(timeToDisplay);
        _timerText.text = timeSpan.ToString(@"ss\:ff");
    }

    private void RestartTimer()
    {
        timeToDisplay = initialTime; // Reset the timer
        _timerText.enabled = true; // Show the text again
        _isRunning = true; // Restart the timer
    }
}
