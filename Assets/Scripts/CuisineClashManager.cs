using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.CompilerServices;
using System.Linq;

public class CuisineClashManager : NetworkBehaviour
{
	public static CuisineClashManager Instance { get; private set; }

	[SerializeField] private Transform playerPrefab;
	private GamemodeManager gamemodeManager;

	private Dictionary<ulong, bool> playerReadyDictionary;
	private Dictionary<ulong, int> playerPoints;

	[SerializeField] private SpawnManager spawnManager;

	private NetworkVariable<bool> gameStarted = new NetworkVariable<bool>();

	public EventHandler AllPlayerObjectsSpawned;

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
		//DontDestroyOnLoad(gameObject);

		playerReadyDictionary = new Dictionary<ulong, bool>();
		playerPoints = new Dictionary<ulong, int>();

		//gamemodeList = new List<string>();
	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.N))
		{
			//Debug.Log("gamemode list.count: " + gamemodeList.Count);
		}
	}

	public void SetPlayerReady()
	{
		SetPlayerReadyServerRpc();
	}

	public override void OnNetworkSpawn()
	{
		gameStarted.Value = false;
		NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
		gamemodeManager = GameObject.FindGameObjectWithTag("Gamemode Manager").GetComponent<GamemodeManager>();
		if (gamemodeManager != null)
		{
			Debug.Log("gamemode manager not null!");
		}
		//Debug.Log("Awake + gamemodeListInstnatiated: " + gamemodeListInstantiated);
		/*if (IsHost)
		{
			foreach (Gamemode gamemode in Enum.GetValues(typeof(Gamemode)))
			{
				Debug.Log("insantianteing gamemodeList...");
				gamemodeList.Add(gamemode.ToString());
				//gamemodeListInstantiated = true;
			}
		}*/
	}
	public override void OnNetworkDespawn()
	{
		NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
	}

	private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
	{
		if (IsHost)
		{
			//Debug.Log("current scene, # of connected clients" + SceneManager.GetActiveScene().name + ", " + NetworkManager.Singleton.ConnectedClientsIds.Count);
			foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
			{
				Transform playerTransform = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
				playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
				Debug.Log("Spawning Player Object!");
			}

			AllPlayerObjectsSpawned?.Invoke(this, EventArgs.Empty);
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

		if (allClientsReady && playerReadyDictionary.Count == NetworkManager.ConnectedClientsIds.Count && gameStarted.Value == false)
		{
			gameStarted.Value = true;
			string gamemode = gamemodeManager.GamemodeSelector();
			Loader.LoadNetwork(gamemode);
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

}
