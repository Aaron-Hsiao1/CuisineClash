using System.Collections;
using UnityEngine;
using TMPro;

public class GGCountdownTimer : MonoBehaviour
{
    public TMP_Text countdownText; // Assign in Inspector
    public int countdownSeconds = 5; // Countdown duration

    private void Start()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        int countdown = countdownSeconds;

        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);

        GoKartMovement.canMove = true; 
    }
}
