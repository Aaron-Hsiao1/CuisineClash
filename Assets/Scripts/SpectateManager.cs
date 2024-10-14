using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class SpectateManager : NetworkBehaviour
{
	public SpectateManager Instance { get; private set; }

	private List<ulong> alivePlayers;

	private Dictionary<ulong, NetworkClient> connectedClients;

	[SerializeField] private Camera spectatorCamera;

	private void Awake()
	{
		Instance = this;

		alivePlayers = new List<ulong>();

		Debug.Log("is rainingmeatballmanager nulLL: " + RainingMeatballManager.Instance == null);

	}
	public override void OnNetworkSpawn()
	{
		Debug.Log("SpectateManager is running on server. Starting event subscription retry.");
		StartCoroutine(SubscribeToDeathZoneEvent());

		if (IsHost)
		{
			Debug.Log("server");
			foreach (var player in NetworkManager.ConnectedClientsIds)
			{
				Debug.Log("for loop");
				var playerData = CuisineClashMultiplayer.Instance.GetPlayerDataFromClientId(player);

				if (playerData.isAlive == true)
				{
					alivePlayers.Add(player);
					Debug.Log("player added to alive players " + playerData.playerName.ToString());
				}
			}
		}

	}

	private IEnumerator SubscribeToDeathZoneEvent()
	{
		while (DeathZone.Instance == null)
		{
			Debug.Log("Waiting for DeathZone instance to be initialized...");
			yield return new WaitForSeconds(0.5f); // Retry every half second
		}

		Debug.Log("DeathZone instance found. Subscribing to OnAlivePlayersChanged.");
		DeathZone.Instance.OnAlivePlayersChanged += SpectateManager_OnAlivePlayersChanged;
	}

	private void SpectateManager_OnAlivePlayersChanged(object sender, EventArgs e)
	{
		if (IsHost)
		{
			Debug.Log("alive playertschanged 2");
			alivePlayers.Clear();
			foreach (var player in NetworkManager.ConnectedClientsIds)
			{
				var playerData = CuisineClashMultiplayer.Instance.GetPlayerDataFromClientId(player);

				if (playerData.isAlive == true)
				{
					alivePlayers.Add(player);
					Debug.Log("player added: " + playerData.playerName);
				}

			}
		}

	}

	private ulong GetFirstSpectatablePlayer()
	{
		if (alivePlayers.Count > 0)
		{
			return alivePlayers[0];
		}
		else
		{
			Debug.LogWarning("alivePlayers.Count == 0");
			return 0;
		}

	}

	public void BecomeSpectator(ulong clientId)
	{
		BecomeSpectatorServerRpc(clientId);
	}

	[ServerRpc(RequireOwnership = false)]
	private void BecomeSpectatorServerRpc(ulong clientId)
	{
		if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient networkClient))
		{
			GameObject playerObject = networkClient.PlayerObject.gameObject;

			playerObject.GetComponentInChildren<Camera>().gameObject.SetActive(false);

			ulong targetClientId = GetFirstSpectatablePlayer();

			SpawnSpectatorCameraForPlayerClientRpc(clientId, targetClientId);
			Debug.Log("you are now a spectator"); //make the spectator's camera = to the spectated player
		}
	}

	[ClientRpc]
	public void SpawnSpectatorCameraForPlayerClientRpc(ulong clientId, ulong targetClientId)
	{
		SpawnSpectatorCameraForPlayer(clientId, targetClientId);
	}

	public void SpawnSpectatorCameraForPlayer(ulong spectatorClientId, ulong targetClientId)
	{
		if (NetworkManager.Singleton.LocalClientId == spectatorClientId)
		{
			RequestTargetCameraDataServerRpc(targetClientId, spectatorClientId);
		}

	}

	[ServerRpc(RequireOwnership = false)]
	private void RequestTargetCameraDataServerRpc(ulong targetClientId, ulong spectatorClientId)
	{
		RequestTargetCameraData(targetClientId, spectatorClientId);
	}

	public void RequestTargetCameraData(ulong targetClientId, ulong spectatorClientId)
	{
		var targetPlayerObject = NetworkManager.Singleton.ConnectedClients[targetClientId].PlayerObject;

		if (targetPlayerObject != null)
		{
			// Find the camera component on the target player object
			Camera targetCamera = targetPlayerObject.transform.Find("CameraHolder").Find("Main Camera").GetComponent<Camera>();

			if (targetCamera != null)
			{
				// Send the target camera's position and rotation back to the client
				SendTargetCameraDataClientRpc(targetClientId, spectatorClientId, targetCamera.transform.position, targetCamera.transform.rotation);
			}
		}
	}

	[ClientRpc]
	private void SendTargetCameraDataClientRpc(ulong targetClientId, ulong spectatorClientId, Vector3 cameraPosition, Quaternion cameraRotation)
	{
		SendTargetCameraData(targetClientId, spectatorClientId, cameraPosition, cameraRotation);
	}

	private void SendTargetCameraData(ulong targetClientId, ulong spectatorClientId, Vector3 cameraPosition, Quaternion cameraRotation)
	{
		if (NetworkManager.Singleton.LocalClientId == spectatorClientId)
		{
			SpawnSpectatorCameraServerRpc(spectatorClientId, cameraPosition, cameraRotation);

			StartCoroutine(CheckForSpectatorCamera(targetClientId, spectatorClientId, cameraPosition, cameraRotation));

		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void SpawnSpectatorCameraServerRpc(ulong spectatorClientId, Vector3 cameraPosition, Quaternion cameraRotation)
	{
		var spectatorCam = Instantiate(spectatorCamera, cameraPosition, cameraRotation);
		var spectatorCamNetworkObject = spectatorCam.GetComponent<NetworkObject>();
		spectatorCamNetworkObject.SpawnWithOwnership(spectatorClientId, true);

		SetCameraNameClientRpc(spectatorClientId, spectatorCamNetworkObject.NetworkObjectId, $"Spectator Camera for {spectatorClientId}");

		if (NetworkManager.LocalClientId != spectatorClientId)
		{
			Destroy(spectatorCam);
		}
	}

	[ClientRpc]
	private void SetCameraNameClientRpc(ulong spectatorClientId, ulong networkObjectId, string newName)
	{
		if (NetworkManager.Singleton.LocalClientId == spectatorClientId)
		{
			// Find the camera object by NetworkObjectId
			var spectatorCam = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject;

			// Set the name on the client
			spectatorCam.name = newName;

			Debug.Log($"Spectator camera name set to: {newName} for client {spectatorClientId}");
		}
	}

	private IEnumerator CheckForSpectatorCamera(ulong targetClientId, ulong spectatorClientId, Vector3 cameraPosition, Quaternion cameraRotation)
	{
		GameObject spectatorCam = null;

		// Wait for the camera object to exist
		while (spectatorCam == null)
		{
			spectatorCam = GameObject.Find("Spectator Camera for " + spectatorClientId);
			yield return null; // Wait for next frame
		}

		// Once found, set the position and rotation
		spectatorCam.GetComponent<SpectatorCamera>().SetTargetClientId(targetClientId);
		Debug.Log("Spectator camera found and updated.");
	}


	// Update is called once per frame
	void Update()
	{

	}
}

