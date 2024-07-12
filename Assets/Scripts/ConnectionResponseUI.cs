using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI messageText;
	[SerializeField] private Button closeButton;

	private void Awake()
	{
		closeButton.onClick.AddListener(Hide);
	}

	private void Start()
	{
		CuisineClashMultiplayer.Instance.OnFailedToJoinGame += CuisineClashMultiplayer_OnFailedToJoinGame;

		Hide();
	}

	private void CuisineClashMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
	{
		Show();

		messageText.text = NetworkManager.Singleton.DisconnectReason;
		if (messageText.text == "")
		{
			messageText.text = "Failed to connect.";
		}
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
		CuisineClashMultiplayer.Instance.OnFailedToJoinGame -= CuisineClashMultiplayer_OnFailedToJoinGame;
	}

}
