using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Player : NetworkBehaviour
{
	public static Player LocalInstance { get; private set; }

	[SerializeField] private PlayerVisual playerVisual;
	[SerializeField] private TextMeshPro playerName;

	[SerializeField] private Rigidbody rb;

	[SerializeField] private ulong localClientId;
	private NetworkVariable<ulong> clientId = new NetworkVariable<ulong>();

	public override void OnNetworkSpawn()
	{
		Debug.Log("player sapwned on server" + NetworkManager.Singleton.LocalClientId);
		//PlayerSpawnFix();
		if (IsLocalPlayer)
		{
			LocalInstance = this;
			localClientId = NetworkManager.Singleton.LocalClientId;
		}
		if (IsOwner)
		{
			clientId.Value = OwnerClientId;
		}
		if (IsServer)
		{
			NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
		}
	}

	private void Update()
	{
		//Debug.Log("Player position: " + transform.position.ToString());
	}

	// Start is called before the first frame update
	void Start()
	{
		if (IsLocalPlayer)
		{
			playerName.gameObject.SetActive(false);
		}
		PlayerData playerData = CuisineClashMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
		playerVisual.SetPlayerColor(CuisineClashMultiplayer.Instance.GetPlayerOuterColor(playerData.outerColorId), CuisineClashMultiplayer.Instance.GetPlayerInnerColor(playerData.innerColorId));
		playerName.text = playerData.playerName.ToString();
	}

	[ServerRpc(RequireOwnership = false)]
	private void SetLocalPlayerClientIdServerRpc(ulong clientId)
	{
		this.clientId.Value = clientId;
	}

	private void PlayerSpawnFix()
	{
		PlayerSpawnFixServerRpc();
	}

	[ServerRpc(RequireOwnership = false)]
	private void PlayerSpawnFixServerRpc()
	{
		//Debug.Log("Player spawned on server, IsServer == true");
		//Debug.Log(NetworkManager.Singleton.LocalClientId);
		SpawnManager spawnManager = GameObject.Find("Spawn Points").GetComponent<SpawnManager>();
		Vector3 nextSpawnPoint = spawnManager.GetNextSpawnPoint();
		SetPlayerLocation(nextSpawnPoint.x, nextSpawnPoint.y, nextSpawnPoint.z);
		//Debug.Log($"Transfrom.position: {rb.position}");
	}

	[ClientRpc]
	private void SetPlayerLocationClientRpc(float x, float y, float z)
	{
		rb.position = new Vector3(x, y, z);
		//Debug.Log($"rb.psition client: {rb.position}");
	}

	public void SetPlayerLocation(float x, float y, float z)
	{
		SetPlayerLocationClientRpc(x, y, z);
	}

	public void KillPlayer(float x, float y, float z)
	{
		KillPlayerClientRpc(x, y, z);
	}

	[ClientRpc]
	private void KillPlayerClientRpc(float x, float y, float z)
	{
		rb.position = new Vector3(x, y, z);
		Debug.Log("New Position Set");
		StartCoroutine(DeactivateAfterFrame());
	}

	private IEnumerator DeactivateAfterFrame()
	{
		yield return null; // Wait for one frame to ensure position update
		transform.gameObject.SetActive(false);
	}

	private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
	{
		if (clientId == OwnerClientId)
		{
			Debug.Log("player " + clientId + " disconnected");
		}
	}

	public ulong GetClientId()
	{
		return OwnerClientId;
	}

	public string GetPlayerName()
	{
		return playerName.text;
	}
}

