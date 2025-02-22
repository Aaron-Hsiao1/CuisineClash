using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;

public class CuisineClashManager : NetworkBehaviour
{
    public static CuisineClashManager Instance { get; private set; }

    [SerializeField] private Transform playerPrefab;   // Default player prefab
    [SerializeField] private Transform goKartPrefab;   // GoKart player prefab for "GoGurt" scene
    [SerializeField] private SpawnManager spawnManager;

    private GamemodeManager gamemodeManager;
    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, int> playerPoints;
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
        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPoints = new Dictionary<ulong, int>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.N))
        {
            // Debugging purposes
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
    }

    void OnDisable()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SceneManager_OnLoadEventCompleted;
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (IsHost)
        {
            Debug.Log("Client list count that finished loading: " + clientsCompleted.Count);

            foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                Vector3 nextSpawnPoint = spawnManager.GetNextSpawnPoint();

                // Select the correct prefab based on the scene
                Transform prefabToSpawn = sceneName == "GoGurt" ? goKartPrefab : playerPrefab;

                // Spawn the correct player prefab
                Transform playerTransform = Instantiate(prefabToSpawn, nextSpawnPoint, Quaternion.identity);
                NetworkObject playerTransformNetwork = playerTransform.GetComponent<NetworkObject>();
                playerTransformNetwork.SpawnAsPlayerObject(clientId, true);

                // Set player position
                playerTransform.gameObject.GetComponent<Player>().SetPlayerLocation(nextSpawnPoint.x, nextSpawnPoint.y, nextSpawnPoint.z);

                Debug.Log($"Spawning {sceneName} Player Object!");
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
