using UnityEngine;
using TMPro; 

public class CountdownTimer : MonoBehaviour
{
    public float startTime = 60.0f; 
    private float currentTime;
    public TMP_Text timerText;
    public bool timerRunning = false;
    public TMP_Text gameOverText; 
    
    void Start()
    {
        gameOverText = GameObject.Find("GameOverText2").GetComponent<TMP_Text>();
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
        currentTime = startTime;
        timerRunning = true;
    }

    void Update()
    {
        if (timerRunning)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                timerRunning = false;
                OnTimerEnd();
            }

            UpdateTimerText();
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void OnTimerEnd()
    {
        ShowGameOverText();
        Destroy(gameObject);
    }

    public void StartTimer(float newTime)
    {
        startTime = newTime;
        currentTime = startTime;
        timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    private void ShowGameOverText()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
        }
        
    }
}
