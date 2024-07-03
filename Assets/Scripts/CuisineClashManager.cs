using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CuisineClashManager : NetworkBehaviour
{
	public static CuisineClashManager Instance { get; private set; }

	[SerializeField] private Transform playerPrefab;

	private enum State
	{
		WaitingToStart,
		InConnectionLobby,
		GamePlaying,
		GameOver,
	}

	private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);

	public override void OnNetworkSpawn()
	{
		NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
	}

	private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
	{
		if (IsHost)
		{
			foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
			{
				Transform playerTransform = Instantiate(playerPrefab);
				playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
			}
		}
	}

	public bool IsWaitingToStart()
	{
		return state.Value == State.WaitingToStart;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.C))
		{
			foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
			{
				Debug.Log("client: " + clientId);
			}
		}
	}
}
