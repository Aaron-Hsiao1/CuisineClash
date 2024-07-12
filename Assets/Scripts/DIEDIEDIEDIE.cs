using UnityEngine;
using TMPro;

public class DeathZone : MonoBehaviour
{
	private TMP_Text gameOverText;

	private void Start()
	{
		gameOverText = GameObject.Find("GameOverText").GetComponent<TMP_Text>();
		if (gameOverText != null)
		{
			gameOverText.gameObject.SetActive(false);
		}
	}

	private void OnTriggerEnter(Collider touch)
	{
		if (touch.CompareTag("DeathZone"))
		{
			Destroy(gameObject);
			ShowGameOverText();
		}
	}

	private void ShowGameOverText()
	{
		if (gameOverText != null)
		{
			Debug.Log("Player Died");
			gameOverText.gameObject.SetActive(true);
		}

	}
}
