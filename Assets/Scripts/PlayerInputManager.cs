using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
	public static bool gamePaused;
	public static bool canPause;
	// Start is called before the first frame update
	void Start()
	{
		gamePaused = false;
		canPause = true;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.P) && !gamePaused && canPause)
		{
			Debug.Log("paused");
			Time.timeScale = 0;
			gamePaused = true;
			StartCoroutine(PauseCD());

		}
		if (Input.GetKeyDown(KeyCode.P) && gamePaused && canPause)
		{
			Debug.Log("unpaused");
			Time.timeScale = 1;
			gamePaused = false;
			StartCoroutine(PauseCD());
		}
	}

	IEnumerator PauseCD()
	{
		canPause = false;
		Debug.Log("canpause = false");
		yield return new WaitForSecondsRealtime(0.5f);
		Debug.Log("canpause = true");
		canPause = true;
	}
}
