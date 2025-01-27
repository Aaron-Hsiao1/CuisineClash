using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;
using System;
using Unity.Mathematics;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;

public class CaptureTheCakeManager : NetworkBehaviour //candycane bonk and m and m shooter banan car hotdog car
{
    [SerializeField] private SpectateManager spectateManager;

    [Header("Teams")]
    private List<ulong> players = new List<ulong>();
    private Dictionary<ulong, int> playerTeams = new Dictionary<ulong, int>(); //0 is team 1, 1 is team 2
    private Dictionary<int, List<ulong>> teams = new Dictionary<int, List<ulong>>()
    {
        { 0, new List<ulong>() }, // Team 1
        { 1, new List<ulong>() }  // Team 2
    };

    [Header("Spawning")]
    [SerializeField] private GameObject team0Spawn;
    [SerializeField] private GameObject team1Spawn;
    private SpawnPoint[] team0SpawnPoints;
    private SpawnPoint[] team1SpawnPoints;
    [SerializeField] private Transform playerPrefab;
    [SerializeField] private SpawnManager spawnManager;
    private float respawnTimer = 3f;

    [Header("End Game UI")]
    [SerializeField] private Camera secondaryCamera;
    [SerializeField] private TMP_Text gameOverText;
    private CuisineClashMultiplayer cuisineClashMultiplayer;
    [SerializeField] private GameObject leaderboard;
    [SerializeField] private TMP_Text leaderboardText;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsHost)
        {
            return;
        }
        Debug.Log("on start");

        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            players.Add(clientId);
        }
        //shufle list
    }

    public override void OnNetworkSpawn()
    {
        CuisineClashManager.Instance.AllPlayerObjectsSpawned += CaptureTheCakeManager_AllPlayerObjectsSpawned;
        cuisineClashMultiplayer = GameObject.Find("CuisineClashMultiplayer").GetComponent<CuisineClashMultiplayer>();
        team0SpawnPoints = team0Spawn.GetComponentsInChildren<SpawnPoint>();
        team1SpawnPoints= team1Spawn.GetComponentsInChildren<SpawnPoint>();
    }

    private void OnDisable()
    {
        CuisineClashManager.Instance.AllPlayerObjectsSpawned -= CaptureTheCakeManager_AllPlayerObjectsSpawned;
    }

    private void CaptureTheCakeManager_AllPlayerObjectsSpawned(object sender, EventArgs e)
    {
        if (!IsHost)
        {
            return;
        }
        System.Random random = new System.Random();

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            // Determine which team has fewer members
            int team = teams[0].Count <= teams[1].Count ? 0 : 1;

            // Assign the player to the team
            teams[team].Add(clientId);
            playerTeams[clientId] = team;

            Debug.Log($"Client {clientId} assigned to team {team}");

            NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject.GetComponent<CaptureTheCakePlayerManager>().SetTeam(team);

            playerTeams[clientId] = team;
        }
        StartCoroutine(StartPlayerSpawns());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void KillPlayerServerRpc(ulong clientId)
    {
        Debug.Log("Kill player server rpc");
        GameObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject;
        NetworkObject playerNetworkObject = playerObject.GetComponent<NetworkObject>();

        playerObject.SetActive(false);

        HidePlayerClientRpc(playerNetworkObject.NetworkObjectId);
        StartSpectatingClientRpc(clientId);
        StartCoroutine(RespawnPlayer(clientId)); //client is not able to spectate??
    }

    [ClientRpc]
    private void HidePlayerClientRpc(ulong networkObjectId) //consider moving to cuisineclashmanager or cuisineclashmultiplayer
    {
        NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject.SetActive(false);
    }

    [ClientRpc]
    private void ShowPlayerClientRpc(ulong networkObjectId) {
        Debug.Log("Shopw player lcinet rpc");
        NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId].gameObject.SetActive(true);
    }

    [ClientRpc]
    private void StartSpectatingClientRpc(ulong clientId)
    {
        
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            Debug.Log(" start spectating client rpc cc");
            spectateManager.RemovePlayerFromSpectatingList(clientId);
            spectateManager.StartSpectating();
        }
    }

    [ClientRpc]
    private void StopSpectatingClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            spectateManager.AddPlayerToSpectatingList(clientId);
            spectateManager.StopSpectating();
        }
    }

    public IEnumerator RespawnPlayer(ulong clientId)
    {
        Debug.Log("Respawning Player...");
        yield return new WaitForSeconds(respawnTimer);
        Debug.Log("Timer Over");
        RespawnPlayerServerRpc(clientId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RespawnPlayerServerRpc(ulong clientId)
    {
        Debug.Log($"Respawning player {clientId}");
        GameObject playerObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject;
        NetworkObject playerNetworkObject = playerObject.GetComponent<NetworkObject>();

        playerObject.SetActive(true);

        Debug.Log("Show player client rpc networkobehjct id: " + playerNetworkObject.NetworkObjectId);
        ShowPlayerClientRpc(playerNetworkObject.NetworkObjectId);
        StopSpectatingClientRpc(clientId);
        Debug.Log("Spawning Player Object!");
        if (playerTeams[clientId] == 0)
        {
            Debug.Log("Player si on team 0");
            Vector3 nextSpawnPoint = GetNextSpawnPointTeam0(false);
            playerObject.GetComponent<Player>().SetPlayerLocation(nextSpawnPoint.x, nextSpawnPoint.y, nextSpawnPoint.z);
        }
        else if (playerTeams[clientId] == 1)
        {
            Debug.Log("Player is on team 1");
            Vector3 nextSpawnPoint = GetNextSpawnPointTeam1(false);
            playerObject.GetComponent<Player>().SetPlayerLocation(nextSpawnPoint.x, nextSpawnPoint.y, nextSpawnPoint.z);
        }
    }

    private IEnumerator StartPlayerSpawns()
    {
        yield return new WaitForSeconds(5f);
        SpawnPlayer();

    }
    private void SpawnPlayer()
    {
        foreach (ulong player in teams[0])
        {
            Vector3 nextSpawnPoint = GetNextSpawnPointTeam0(true);
            GameObject playerGameObject = NetworkManager.Singleton.ConnectedClients[player].PlayerObject.gameObject;
            playerGameObject.GetComponent<Player>().SetPlayerLocation(nextSpawnPoint.x, nextSpawnPoint.y, nextSpawnPoint.z);
        }
        foreach (ulong player in teams[1])
        {
            Vector3 nextSpawnPoint = GetNextSpawnPointTeam1(true);
            GameObject playerGameObject = NetworkManager.Singleton.ConnectedClients[player].PlayerObject.gameObject;
            playerGameObject.GetComponent<Player>().SetPlayerLocation(nextSpawnPoint.x, nextSpawnPoint.y, nextSpawnPoint.z);
        }
    }

    private Vector3 GetNextSpawnPoint(SpawnPoint[] spawnPoints, bool startOfGame)
    {
        foreach (var spawnPoint in spawnPoints)
        {
            if (startOfGame)
            {
                if (spawnPoint.isAvailable)
                {
                    spawnPoint.isAvailable = false;
                    Vector3 _tempSpawnPoint = spawnPoint.transform.position + new Vector3(0, 2, 0);
                    return _tempSpawnPoint;
                }
            }
            else
            {
                Vector3 _tempSpawnPoint = spawnPoint.transform.position + new Vector3(0, 2, 0);
                return _tempSpawnPoint;
            }
        }
        // Fallback if no spawn points are available
        return new Vector3(0, 0, 0);
    }

    private Vector3 GetNextSpawnPointTeam0(bool startOfGame)
    {
        return GetNextSpawnPoint(team0SpawnPoints, startOfGame);
    }

    private Vector3 GetNextSpawnPointTeam1(bool startOfGame)
    {
        return GetNextSpawnPoint(team1SpawnPoints, startOfGame);
    }

    [ClientRpc]
    public void EndGameClientRpc(int winningTeam)
    {
        CalculatePoints(winningTeam);
        EndGame();
        Debug.Log("timer.gameEnded()");
    }

    private void CalculatePoints(int winningTeam)
    {
        foreach(ulong clientId in teams[winningTeam])
        {
            cuisineClashMultiplayer.AddPoints(clientId, 3);
        }
    }

    public void EndGame()
    {
        StartCoroutine(ShowEndGameUIs());
    }

    IEnumerator ShowEndGameUIs()
    {
        Cursor.lockState = CursorLockMode.None;
        secondaryCamera.gameObject.SetActive(true);
        gameOverText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        gameOverText.gameObject.SetActive(false);
        UpdateLeaderboardClientRpc();
        leaderboard.SetActive(true);

        yield return new WaitForSeconds(3f);

        if (GamemodeManager.Instance.GetGamemodeList().Count > 0)
        {
            Loader.LoadNetwork(Loader.Scene.PregameLobby.ToString());
        }
        if (GamemodeManager.Instance.GetGamemodeList().Count == 0)
        {
            Loader.LoadNetwork(Loader.Scene.GameEnded.ToString());
        }
    }

    private void UpdateLeaderboard()
    {
        Debug.Log("updating leaderboard...");
        foreach (KeyValuePair<ulong, int> player in cuisineClashMultiplayer.GetPlayerPoints())
        {
            var playerName = CuisineClashMultiplayer.Instance.GetPlayerDataFromClientId(player.Key).playerName;
            leaderboardText.text += $"{playerName}: {player.Value}\n";
        }
        Debug.Log($"leaderboradString: {leaderboardText.text}");
        //leaderboardText.text = leaderboardString.Value.ToString();
    }

    [ClientRpc]
    private void UpdateLeaderboardClientRpc()
    {
        UpdateLeaderboard();
    }
}
