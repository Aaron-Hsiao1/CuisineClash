using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CuisineClashMultiplayer : NetworkBehaviour
{
	private const int MAX_PLAYER_AMOUNT = 2;

	public static CuisineClashMultiplayer Instance { get; private set; }

	public event EventHandler OnTryingToJoinGame;
	public event EventHandler OnFailedToJoinGame;

	private void Awake()
	{
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void StartHost()
	{
		NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
		NetworkManager.Singleton.StartHost();

	}

	private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
	{
		if (SceneManager.GetActiveScene().name != Loader.Scene.ConnectionLobby.ToString())
		{
			connectionApprovalResponse.Approved = false;
			connectionApprovalResponse.Reason = "Game has already started.";
			return;
		}

		if (NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
		{
			connectionApprovalResponse.Approved = false;
			connectionApprovalResponse.Reason = "Game is full.";
			return;
		}

		connectionApprovalResponse.Approved = true;

	}
	public void StartClient()
	{
		OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);
		NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
		NetworkManager.Singleton.StartClient();
	}

	private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
	{
		OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
	}

}
