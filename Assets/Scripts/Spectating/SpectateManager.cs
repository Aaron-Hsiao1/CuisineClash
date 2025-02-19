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

					currentSpectatorCamera.SetActive(false);
					currentSpectatorFreeLookCamera.SetActive(false);

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

					currentSpectatorCamera.SetActive(false);
					currentSpectatorFreeLookCamera.SetActive(false);

					SpectatePlayer(currentPlayerBeingSpectated);
				}
			}
		}
	}

	public void StartSpectating()
	{
		Debug.Log("start spectagin");
		isSpectating = true;
        currentIndexBeingSpectated = 0;
        currentPlayerBeingSpectated = availableToSpectateList[currentIndexBeingSpectated];
        SpectatePlayer(currentPlayerBeingSpectated);
    }

	public void StopSpectating()
	{
		isSpectating = false;
        currentSpectatorCamera.SetActive(false);
        currentSpectatorFreeLookCamera.SetActive(false);
    }

	/*
	[ClientRpc]
	private void StopSpectatingClientRpc(ulong spectatorClientId)
	{
		if (NetworkManager.Singleton.LocalClientId != spectatorClientId)
        {
            return;
        }
		currentSpectatorCamera.SetActive(false);
		currentSpectatorFreeLookCamera.SetActive(false);
    }

	[ClientRpc]
	private void StartSpectatingClientRpc(ulong spectatorClientId)
	{
        Debug.Log("start spectating client rpc before if");

        if (NetworkManager.Singleton.LocalClientId != spectatorClientId)
		{
			return;
		}
		Debug.Log("start spectating client rpc");
		currentIndexBeingSpectated = 0;
		currentPlayerBeingSpectated = availableToSpectateList[currentIndexBeingSpectated];
		SpectatePlayer(currentPlayerBeingSpectated);
	}*/

	public void RemovePlayerFromSpectatingList(ulong spectatorClientId)
	{
		RemovePlayerFromSpectatingListServerRpc(spectatorClientId);
	}

	public void AddPlayerToSpectatingList(ulong spectatorClientId)
	{
		AddPlayerToSpectatingListServerRpc(spectatorClientId);

    }

	[ServerRpc(RequireOwnership = false)]
	private void AddPlayerToSpectatingListServerRpc(ulong spectatorClientId)
	{
		availableToSpectateList.Add(spectatorClientId);
	}

	[ServerRpc(RequireOwnership = false)]
	private void RemovePlayerFromSpectatingListServerRpc(ulong spectatorClientId)
	{
		availableToSpectateList.Remove(spectatorClientId);
	}


	public void SpectatePlayer(ulong spectatorClientId)
	{
		Debug.Log("Spectate Player");
		SpectatePlayerServerRpc(spectatorClientId);
    }

	[ClientRpc]
	private void SpectatePlayerClientRpc(ulong networkObjectId, ulong recieverId)
	{
		if (NetworkManager.Singleton.LocalClientId == recieverId)
		{
			Debug.Log("spectate player client rpc");
			GameObject playerGameObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject;
			currentSpectatorCamera = playerGameObject.transform.Find("CameraHolder/Main Camera").gameObject;
			currentSpectatorFreeLookCamera = playerGameObject.transform.Find("FreeLook Camera").gameObject;

			currentSpectatorCamera.SetActive(true);
			currentSpectatorFreeLookCamera.SetActive(true);
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void SpectatePlayerServerRpc(ulong clientId, ServerRpcParams serverRpcParams = default)
	{
		Debug.Log("Spectate player sever rpc");
		var senderId = serverRpcParams.Receive.SenderClientId;
		ulong networkObjectId = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject.GetComponent<NetworkObject>().NetworkObjectId;

		SpectatePlayerClientRpc(networkObjectId, senderId);
	}
}
