using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class SpectateManager : NetworkBehaviour
{
	private NetworkList<ulong> availableToSpectateList;
	public bool isSpectating = false;

	private GameObject currentSpectatorCamera;
	private GameObject currentSpectatorFreeLookCamera;

	public ulong currentPlayerBeingSpectated;
	public int currentIndexBeingSpectated;

	private void Awake()
	{
		availableToSpectateList = new NetworkList<ulong>();
	}

	public override void OnNetworkSpawn()
	{
		if (!IsHost)
		{
			return;
		}

		foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
		{
			availableToSpectateList.Add(clientId);
		}
		Debug.Log("available to spectate list ocunt: " + availableToSpectateList.Count);
	}

	private void Update()
	{
		if (isSpectating)
		{
			if (Input.GetKeyDown(KeyCode.Mouse0)) //left mouse button
			{
				Debug.Log("lmb pressed");
				Debug.Log("available to spectate count: " + availableToSpectateList.Count);
				if (currentIndexBeingSpectated == 0)
				{
					return;
				}
				else
				{
					Debug.Log("switching players");
					currentIndexBeingSpectated--;
					currentPlayerBeingSpectated = availableToSpectateList[currentIndexBeingSpectated];

					currentSpectatorCamera.gameObject.SetActive(false);
					currentSpectatorFreeLookCamera.gameObject.SetActive(false);

					SpectatePlayer(currentPlayerBeingSpectated);
				}
			}
			if (Input.GetKeyDown(KeyCode.Mouse1))//right mouse button
			{
				Debug.Log("rmb pressed");
				Debug.Log("available to spectate count: " + availableToSpectateList.Count);
				if (currentIndexBeingSpectated == availableToSpectateList.Count - 1)
				{
					return;
				}
				else
				{
					Debug.Log("switching players");
					currentIndexBeingSpectated++;
					currentPlayerBeingSpectated = availableToSpectateList[currentIndexBeingSpectated];

					currentSpectatorCamera.gameObject.SetActive(false);
					currentSpectatorFreeLookCamera.gameObject.SetActive(false);

					SpectatePlayer(currentPlayerBeingSpectated);
				}
			}
		}
	}

	public void StartSpectating(ulong spectator)
	{
		isSpectating = true;
        Debug.Log("Starting spectating sequ4ence");
		if (!IsHost)
		{
			StartSpectatingServerRpc(spectator);
			return;

        }
        StartSpectatingClientRpc(spectator);
	}

	[ServerRpc(RequireOwnership = false)]
	private void StartSpectatingServerRpc(ulong spectator)
	{
		StartSpectatingClientRpc(spectator);
	}

	[ClientRpc]
	private void StartSpectatingClientRpc(ulong spectator)
	{
		if (NetworkManager.Singleton.LocalClientId != spectator)
		{
			return;
		}
		
		currentIndexBeingSpectated = 0;
		currentPlayerBeingSpectated = availableToSpectateList[currentIndexBeingSpectated];
		SpectatePlayer(currentPlayerBeingSpectated);
	}

	public void RemovePlayerFromSpectatingList(ulong clientId)
	{
		RemovePlayerFromSpectatingListServerRpc(clientId);
	}

	[ServerRpc(RequireOwnership = false)]
	private void RemovePlayerFromSpectatingListServerRpc(ulong clientId)
	{
		availableToSpectateList.Remove(clientId);
	}


	public void SpectatePlayer(ulong clientId)
	{
		GetPlayerPrefabNetworkObjectIdFromClientId(clientId);
	}

	public void GetPlayerPrefabNetworkObjectIdFromClientId(ulong clientId)
	{
		SpectatePlayerServerRpc(clientId);
	}

	[ClientRpc]
	private void SpectatePlayerClientRpc(ulong networkObjectId, ulong recieverId)
	{
		if (NetworkManager.Singleton.LocalClientId == recieverId)
		{
			
			GameObject playerGameObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject;
			Debug.Log($"{recieverId} is currently spectating");
			currentSpectatorCamera = playerGameObject.transform.Find("CameraHolder/Main Camera").gameObject;
			currentSpectatorFreeLookCamera = playerGameObject.transform.Find("FreeLook Camera").gameObject;

			currentSpectatorCamera.SetActive(true);
			currentSpectatorFreeLookCamera.gameObject.SetActive(true);
		}

    }

	[ServerRpc(RequireOwnership = false)]
	private void SpectatePlayerServerRpc(ulong clientId, ServerRpcParams serverRpcParams = default)
	{
		var senderId = serverRpcParams.Receive.SenderClientId;
		ulong networkObjectId = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject.GetComponent<NetworkObject>().NetworkObjectId;

		SpectatePlayerClientRpc(networkObjectId, senderId);
	}
}
