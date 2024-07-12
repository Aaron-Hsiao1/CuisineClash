using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.CompilerServices;

public class CuisineClashManager : NetworkBehaviour
{
	public static CuisineClashManager Instance { get; private set; }

	[SerializeField] private Transform playerPrefab;
	private GamemodeManager gamemodeManager;

	private Dictionary<ulong, bool> playerReadyDictionary;
	//private static List<string> gamemodeList;

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

		//gamemodeList = new List<string>();
	}

	public void SetPlayerReady()
	{
		SetPlayerReadyServerRpc();
	}

	public override void OnNetworkSpawn()
	{
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

	private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
	{
		if (IsHost)
		{
			//Debug.Log("current scene, # of connected clients" + SceneManager.GetActiveScene().name + ", " + NetworkManager.Singleton.ConnectedClientsIds.Count);
			foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
			{
				//Debug.Log("current scene spawned in player" + SceneManager.GetActiveScene().name);
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
			Loader.LoadNetwork(gamemodeManager.gamemodeSelector());
			state.Value = State.GamePlaying;
		}
	}

	/*public Loader.Scene gamemodeSelector()
	{
		Debug.Log("gamemode selectoer rnning");
		if (IsHost)
		{
			int random = UnityEngine.Random.Range(0, gamemodeList.Count); //selects a random index from the list of gamemodes
			string nextGamemode = gamemodeList[random]; //picks the next gamemode based on the index

			foreach (Loader.Scene scene in Enum.GetValues(typeof(Loader.Scene))) //loops through the Loader.Scene to find the scene that matches with the gamemode
			{
				if (scene.ToString() == nextGamemode)
				{
					Debug.Log("random: " + random);
					Debug.Log("gamemode count: " + gamemodeList.Count);
					gamemodeList.RemoveAt(random);
					Debug.Log("gamemode removed!");
					Debug.Log("gamemode count: " + gamemodeList.Count);
					return scene; //loads the gamemode
				}
			}
		}
		return Loader.Scene.MainMenu;
	}*/

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
			//Debug.Log("gamemode list.count: " + gamemodeList.Count);
		}
	}
}
