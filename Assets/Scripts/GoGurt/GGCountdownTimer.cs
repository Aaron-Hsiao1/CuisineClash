using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class GGCountdownTimer : MonoBehaviour
{
   
    [SerializeField] private List<RawImage> CountDown;
    public int countdownSeconds = 5; // Countdown duration

    private void Start()
    {
        for (int i = 0; i < CountDown.Count; i++)
        {
            if (CountDown[i] != null)
            {
                CountDown[i].enabled = false;
                CountDown[i].gameObject.SetActive(false);
            }
        }
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        int countdown = countdownSeconds;

        while (countdown > 0)
        {
            CountDown[countdown - 1].enabled = true;
            CountDown[countdown - 1].gameObject.SetActive(true);
            for (int i = 0; i < CountDown.Count; i++)
            {
                if (i != countdown - 1)
                {
                    CountDown[i].enabled = false;
                    CountDown[i].gameObject.SetActive(false);
                }
            }

            yield return new WaitForSeconds(1f);
            countdown--;
        }
        CountDown[0].enabled = false;
        CountDown[0].gameObject.SetActive(false);
        CountDown[5].enabled = true;
        CountDown[5].gameObject.SetActive(true); 
        yield return new WaitForSeconds(1f);
        CountDown[5].enabled = false;
        CountDown[5].gameObject.SetActive(false);
        GoKartMovement.canMove = true; 
    }
}
