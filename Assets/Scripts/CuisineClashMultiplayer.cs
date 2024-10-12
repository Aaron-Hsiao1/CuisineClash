using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using System.Linq;

public class CuisineClashMultiplayer : NetworkBehaviour
{
	public const int MAX_PLAYER_AMOUNT = 8;
	private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

	private string joinCode;
	private string playerName;

	public static CuisineClashMultiplayer Instance { get; private set; }

	public event EventHandler OnTryingToJoinGame;
	public event EventHandler OnFailedToJoinGame;
	public event EventHandler OnPlayerDataNetworkListChanged;

	[SerializeField] private List<Color> playerColorList;
	private Dictionary<ulong, int> playerPoints;

	private NetworkList<PlayerData> playerDataNetworkList;

	private void Awake()
	{
		Instance = this;
		DontDestroyOnLoad(gameObject);

		playerPoints = new Dictionary<ulong, int>();

		playerDataNetworkList = new NetworkList<PlayerData>();
		playerDataNetworkList.OnListChanged += CuisineClashMultiplayer_OnListChanged;
		playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, $"Player {UnityEngine.Random.Range(0, 10000)}");
	}

	private void CuisineClashMultiplayer_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
	{
		OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
	}

	public string GetPlayerName()
	{
		return playerName;
	}

	public void SetPlayerName(string playerName)
	{
		this.playerName = playerName;

		PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, playerName);
	}


	public void StartHost()
	{
		NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
		NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
		NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
		NetworkManager.Singleton.StartHost();
	}

	private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
	{
		for (int i = 0; i < playerDataNetworkList.Count; i++)
		{
			PlayerData playerData = playerDataNetworkList[i];
			if (playerData.clientId == clientId)
			{
				playerDataNetworkList.RemoveAt(i);
			}
		}
	}

	private void NetworkManager_OnClientConnectedCallback(ulong clientId)
	{
		playerDataNetworkList.Add(new PlayerData
		{
			clientId = clientId,
			colorId = GetFirstUnusedColorId(),
		});
		SetPlayerNameServerRpc(GetPlayerName());
		SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
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

		NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
		NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
		NetworkManager.Singleton.StartClient();
	}

	private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
	{
		SetPlayerNameServerRpc(GetPlayerName());
		SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
	}

	private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
	{
		OnFailedToJoinGame?.Invoke(this, EventArgs.Empty);
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
	{
		int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
		PlayerData playerData = playerDataNetworkList[playerDataIndex];
		playerData.playerName = playerName;
		playerDataNetworkList[playerDataIndex] = playerData;
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
	{
		int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
		PlayerData playerData = playerDataNetworkList[playerDataIndex];
		playerData.playerId = playerId;
		playerDataNetworkList[playerDataIndex] = playerData;
	}

	public void SetJoinCode(string joinCode)
	{
		this.joinCode = joinCode;
	}

	public bool IsPlayerIndexConnected(int playerIndex)
	{
		return playerIndex < playerDataNetworkList.Count;
	}

	public Color getPlayerColor(int colorId)
	{
		return playerColorList[colorId];
	}

	public PlayerData GetPlayerDataFromClientId(ulong clientId)
	{
		foreach (PlayerData playerData in playerDataNetworkList)
		{
			if (playerData.clientId == clientId)
			{
				return playerData;
			}
		}
		return default;
	}

	public int GetPlayerDataIndexFromClientId(ulong clientId)
	{
		for (int i = 0; i < playerDataNetworkList.Count; i++)
		{
			if (playerDataNetworkList[i].clientId == clientId)
			{
				return i;
			}
		}

		return -1;
	}

	public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
	{
		return playerDataNetworkList[playerIndex];
	}

	public PlayerData GetPlayerData()
	{
		return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
	}

	public void ChangePlayerColor(int colorId)
	{
		ChangePlayerColorServerRpc(colorId);
	}

	[ServerRpc(RequireOwnership = false)]
	public void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
	{
		if (!IsColorAvailable(colorId))
		{
			return;
		}

		int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
		PlayerData playerData = playerDataNetworkList[playerDataIndex];
		playerData.colorId = colorId;
		playerDataNetworkList[playerDataIndex] = playerData;
	}

	private bool IsColorAvailable(int colorId)
	{
		foreach (PlayerData playerData in playerDataNetworkList)
		{
			if (playerData.colorId == colorId)
			{
				return false;
			}
		}
		return true;
	}

	private int GetFirstUnusedColorId()
	{
		for (int i = 0; i < playerColorList.Count; i++)
		{
			if (IsColorAvailable(i))
			{
				return i;
			}
		}
		return -1;
	}

	public void KickPlayer(ulong clientId)
	{
		Debug.Log($"Player Kicked: {clientId}");
		NetworkManager.Singleton.DisconnectClient(clientId);
		NetworkManager_Server_OnClientDisconnectCallback(clientId);
	}

	public void AddPoints(ulong playerId, int pointAmt)
	{
		AddPointsClientRpc(playerId, pointAmt);
	}

	[ClientRpc]
	private void AddPointsClientRpc(ulong playerId, int pointAmt)
	{
		if (!playerPoints.ContainsKey(playerId))
		{
			playerPoints.Add(playerId, pointAmt);
		}
		else
		{
			playerPoints[playerId] += pointAmt;
		}
	}

	public void SortPoints()
	{
		SortPointsServerRpc();
	}

	[ServerRpc(RequireOwnership = false)]
	private void SortPointsServerRpc()
	{
		playerPoints = playerPoints.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
	}

	public Dictionary<ulong, int> GetPlayerPoints()
	{
		SortPoints();
		return playerPoints;
	}
}
