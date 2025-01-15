using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using System.Collections;

public class CountDownHP : MonoBehaviour
{
    public TextMeshProUGUI countdownText; // Reference to the TextMeshProUGUI component
    private int countdownTime = 3; // Start time for the countdown

    void Start()
    {
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        while (countdownTime > 0)
        {
            countdownText.text = countdownTime.ToString(); // Display current countdown number
            yield return new WaitForSeconds(1); // Wait for 1 second
            countdownTime--; // Decrease the countdown time
        }

        countdownText.text = "Start!"; // Display "Start!" at the end of the countdown
        yield return new WaitForSeconds(1);

        countdownText.gameObject.SetActive(false); // Hide countdown text after displaying "Start!"
    }
}
