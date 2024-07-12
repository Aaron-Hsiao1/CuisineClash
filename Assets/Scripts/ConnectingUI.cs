using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		CuisineClashMultiplayer.Instance.OnTryingToJoinGame += CuisineClashMultiplayer_OnTryingToJoinGame;
		CuisineClashMultiplayer.Instance.OnFailedToJoinGame += CuisineClashManager_OnFailedToJoinGame;

		Hide();
	}

	private void CuisineClashMultiplayer_OnTryingToJoinGame(object sender, System.EventArgs e)
	{
		Show();
	}

	private void CuisineClashManager_OnFailedToJoinGame(object sender, System.EventArgs e)
	{
		Hide();
	}

	private void Show()
	{
		gameObject.SetActive(true);
	}
	private void Hide()
	{
		gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		CuisineClashMultiplayer.Instance.OnTryingToJoinGame -= CuisineClashMultiplayer_OnTryingToJoinGame;
		CuisineClashMultiplayer.Instance.OnFailedToJoinGame -= CuisineClashManager_OnFailedToJoinGame;
	}
}
