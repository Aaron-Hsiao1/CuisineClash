using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
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
		CuisineClashLobby.Instance.OnCreateLobbyStarted += CuisineClashLobby_OnCreateLobbyStarted;
		CuisineClashLobby.Instance.OnCreateLobbyFailed += CuisineClashLobby_OnCreateLobbyFailed;
		CuisineClashLobby.Instance.OnJoinStarted += CuisineClashLobby_OnJoinStarted;
		CuisineClashLobby.Instance.OnJoinFailed += CuisineClashLobby_OnJoinFailed;
		CuisineClashLobby.Instance.OnQuickJoinFailed += CuisineClashLobby_OnQuickJoinFailed;

		Hide();
	}

	private void CuisineClashLobby_OnQuickJoinFailed(object sender, EventArgs e)
	{
		ShowMessage("Could not find a lobby");
	}

	private void CuisineClashLobby_OnJoinFailed(object sender, EventArgs e)
	{
		ShowMessage("Failed to join lobby");
	}

	private void CuisineClashLobby_OnJoinStarted(object sender, EventArgs e)
	{
		ShowMessage("Joining Lobby...");
	}

	private void CuisineClashLobby_OnCreateLobbyFailed(object sender, EventArgs e)
	{
		ShowMessage("Failed to create lobby");
	}

	private void CuisineClashLobby_OnCreateLobbyStarted(object sender, EventArgs e)
	{
		ShowMessage("Creating Lobby...");
	}

	private void CuisineClashMultiplayer_OnFailedToJoinGame(object sender, EventArgs e)
	{
		if (NetworkManager.Singleton.DisconnectReason == "")
		{
			ShowMessage("Failed to connect");
		}
		else
		{
			ShowMessage(NetworkManager.Singleton.DisconnectReason);
		}

		Show();
	}

	private void ShowMessage(string message)
	{
		Show();
		messageText.text = message;
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
		CuisineClashLobby.Instance.OnCreateLobbyStarted -= CuisineClashLobby_OnCreateLobbyStarted;
		CuisineClashLobby.Instance.OnCreateLobbyFailed -= CuisineClashLobby_OnCreateLobbyFailed;
		CuisineClashLobby.Instance.OnJoinStarted -= CuisineClashLobby_OnJoinStarted;
		CuisineClashLobby.Instance.OnJoinFailed -= CuisineClashLobby_OnJoinFailed;
		CuisineClashLobby.Instance.OnQuickJoinFailed -= CuisineClashLobby_OnQuickJoinFailed;
	}

}
