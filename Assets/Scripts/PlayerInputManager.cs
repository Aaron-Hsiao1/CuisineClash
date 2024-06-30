using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
	public static bool escapeMenuOpen;
	public static bool canEscape;

	public Canvas escapeMenu;
	// Start is called before the first frame update
	void Start()
	{
		escapeMenuOpen = false;
		canEscape = true;

		Cursor.lockState = CursorLockMode.Locked;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !escapeMenuOpen && canEscape)
		{
			Debug.Log("paused");
			escapeMenu.gameObject.SetActive(true);
			Cursor.lockState = CursorLockMode.None;
			escapeMenuOpen = true;
			StartCoroutine(EscapeCD());

		}
		if (Input.GetKeyDown(KeyCode.Escape) && escapeMenuOpen && canEscape)
		{
			Debug.Log("unpaused");
			escapeMenu.gameObject.SetActive(false);
			Cursor.lockState = CursorLockMode.Locked;
			escapeMenuOpen = false;
			StartCoroutine(EscapeCD());
		}
	}

	IEnumerator EscapeCD()
	{
		canEscape = false;
		Debug.Log("canpause = false");
		yield return new WaitForSecondsRealtime(0.5f);
		Debug.Log("canpause = true");
		canEscape = true;
	}
}
