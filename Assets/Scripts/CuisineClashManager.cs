using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class CuisineClashManager : NetworkBehaviour
{
	public static CuisineClashManager Instance { get; private set; }

	[SerializeField] private Transform playerPrefab;

	private Dictionary<ulong, bool> playerReadyDictionary;

	private enum State
	{
		WaitingToStart,
		InConnectionLobby,
		InPregameLobby,
		GamePlaying,
		GameOver,
	}

	private bool isLocalPlayerReady;

	private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);

	private void Awake()
	{
		Instance = this;

		playerReadyDictionary = new Dictionary<ulong, bool>();
	}

	public void SetPlayerReady()
	{
		SetPlayerReadyServerRpc();
	}

	public override void OnNetworkSpawn()
	{
		NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
	}
	public override void OnNetworkDespawn()
	{
		NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
	}

	private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
	{
		if (IsHost)
		{
			Debug.Log("current scene, # of connected clients" + SceneManager.GetActiveScene().name + ", " + NetworkManager.Singleton.ConnectedClientsIds.Count);
			foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
			{
				Debug.Log("current scene spawned in player" + SceneManager.GetActiveScene().name);
				Transform playerTransform = Instantiate(playerPrefab);
				playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
			}
		}
	}

	public bool IsWaitingToStart()
	{
		return state.Value == State.WaitingToStart;
	}
	public bool IsLocalPlayerReady()
	{
		return isLocalPlayerReady;
	}
	public void SetIsLocalPlayerReady()
	{
		isLocalPlayerReady = true;
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
	{
		playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

		bool allClientsReady = true;
		foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
		{
			if (playerReadyDictionary.Count != NetworkManager.Singleton.ConnectedClientsIds.Count || !playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
			{
				// This player is NOT ready
				allClientsReady = false;
				break;
			}
		}

		if (allClientsReady && playerReadyDictionary.Count == NetworkManager.ConnectedClientsIds.Count)
		{
			Loader.LoadNetwork(Loader.Scene.MultiplayerTesting);
			state.Value = State.GamePlaying;
		}
	}

	public void SetPlayerUnready()
	{
		SetPlayerUnreadyServerRpc();
	}
	public void SetIsLocalPlayerUnready()
	{
		isLocalPlayerReady = false;
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetPlayerUnreadyServerRpc(ServerRpcParams serverRpcParams = default)
	{
		playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = false;
	}
	private void Update()
	{
		if (Input.GetKey(KeyCode.N))
		{
			Debug.Log("connectedClientsIds.Count: " + NetworkManager.ConnectedClientsIds.Count);
			Debug.Log("playerReadyDictionary.Count: " + playerReadyDictionary.Count);
		}
	}
}
